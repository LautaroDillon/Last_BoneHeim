using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    public Camera mainCam;

    private Rigidbody rb;
    private PlayerMovement pm;
    private float dashBufferTime = 0.15f;
    private float dashBufferCounter = 0f;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

    [Header("Dash Cancel")]
    public float dashCancelCheckDistance = 1f;
    public LayerMask whatIsGround;

    [Header("CameraEffects")]
    public PlayerCamera cam;
    public float dashFov;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;
    [SerializeField] private Image _dashCdBar;
    private float dashCdtarget;
    private float dashCdReduceSpeed = 100f;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.LeftShift;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (PauseManager.isPaused || PlayerHealth.hasDied)
            return;

        if (Input.GetKeyDown(dashKey))
            dashBufferCounter = dashBufferTime;

        if (dashBufferCounter > 0f)
        {
            dashBufferCounter -= Time.deltaTime;

            if (dashCdTimer <= 0f && pm.canDash)
            {
                Dash();
                dashBufferCounter = 0f;
            }
        }

        DashCooldown();
    }

    private void FixedUpdate()
    {
        if (pm.dashing)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > dashForce)
            {
                Vector3 limitedVel = flatVel.normalized * dashForce;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        if (pm.dashing)
        {
            if (Physics.Raycast(transform.position, GetDirection(useCameraForward ? playerCam : orientation), dashCancelCheckDistance, whatIsGround))
            {
                CancelInvoke(nameof(ResetDash));
                ResetDash();
            }
        }
    }

    private void DashCooldown()
    {
        if (dashCdTimer > 0f)
            dashCdTimer -= Time.deltaTime;

        dashCdtarget = dashCdTimer / dashCd;
        _dashCdBar.fillAmount = Mathf.Lerp(_dashCdBar.fillAmount, dashCdtarget, Time.deltaTime * 10f);
    }

    private void Dash()
    {
        if (dashCdTimer > 0f)
            return;

        dashCdTimer = dashCd;
        StartCoroutine(DashCameraKick());

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        AudioManager.instance.PlaySFXOneShot("Dash Grunt", 1f);
        AudioManager.instance.PlaySFXOneShot("Dash", 1.5f);

        cam.DoFov(mainCam.fieldOfView - dashFov);

        Vector3 direction = GetDirection(useCameraForward ? playerCam : orientation);
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        if (resetVel)
            rb.velocity = Vector3.zero;

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        cam.DoFov(mainCam.fieldOfView + dashFov);

        if (disableGravity)
            rb.useGravity = true;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (allowAllDirections)
        {
            Vector3 dir = forwardT.forward * v + forwardT.right * h;
            return dir == Vector3.zero ? forwardT.forward : dir.normalized;
        }

        return forwardT.forward;
    }

    private IEnumerator DashCameraKick()
    {
        Vector3 originalPos = playerCam.localPosition;
        Vector3 dashOffset = new Vector3(0, -0.1f, -0.2f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            playerCam.localPosition = Vector3.Lerp(originalPos, originalPos + dashOffset, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f;
            playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, originalPos, t);
            yield return null;
        }
    }
}
