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

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float maxDashYSpeed;
    public float dashDuration;

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

        if (Input.GetKeyDown(dashKey) && pm.canDash)
            Dash();

        DashCooldown();
    }

    private void DashCooldown()
    {
        if (dashCdTimer > 0f)
            dashCdTimer -= Time.deltaTime;

        dashCdtarget = dashCdTimer / dashCd;
        _dashCdBar.fillAmount = Mathf.MoveTowards(_dashCdBar.fillAmount, dashCdtarget, dashCdReduceSpeed * Time.deltaTime);
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
