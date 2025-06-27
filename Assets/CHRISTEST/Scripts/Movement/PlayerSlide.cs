using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform cameraHolder;

    private CapsuleCollider cc;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime = 1.2f;
    public float slideForce = 400f;
    private float slideTimer;

    private bool slideCooldown = false;
    public float slideCooldownDuration = 0.5f;

    public float slideYScale = 0.5f;
    private float startYScale;

    [Header("Slide Visual")]
    public GameObject slideVisual;

    [Header("Camera Slide")]
    public float slideCamYPos = 0.5f;
    private float startCamYPos;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        cc = GetComponent<CapsuleCollider>();

        startYScale = cc.height;
        startCamYPos = cameraHolder.localPosition.y;
        slideVisual.SetActive(false);
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 camPos = cameraHolder.localPosition;
        float targetY = pm.sliding ? slideCamYPos : startCamYPos;
        camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * 10f);
        cameraHolder.localPosition = camPos;

        if (PauseManager.isPaused || PlayerHealth.hasDied)
            return;

        if (pm.grounded && !slideCooldown)
        {
            if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            {
                StartSlide();
                StartCoroutine(SlideCooldown());
            }

            if ((Input.GetKeyUp(slideKey) || (horizontalInput == 0 && verticalInput == 0)) && pm.sliding)
            {
                StopSlide();
            }
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void LateUpdate()
    {
        float targetHeight = pm.sliding ? slideYScale : startYScale;
        cc.height = Mathf.Lerp(cc.height, targetHeight, Time.deltaTime * 10f);
    }

    private void StartSlide()
    {
        if (!pm.grounded || pm.sliding)
            return;

        pm.sliding = true;
        slideTimer = maxSlideTime;

        AudioManager.instance.PlaySFX("Slide", 1f, false);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Stick to ground

        if (slideVisual != null)
            slideVisual.SetActive(true);
    }

    private void StopSlide()
    {
        if (!pm.sliding)
            return;

        pm.sliding = false;
        AudioManager.instance.StopSFX("Slide");

        if (slideVisual != null)
            slideVisual.SetActive(false);
    }

    private IEnumerator SlideCooldown()
    {
        slideCooldown = true;
        yield return new WaitForSeconds(slideCooldownDuration);
        slideCooldown = false;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce * Time.fixedDeltaTime, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce * Time.fixedDeltaTime, ForceMode.Force);
        }

        if (slideTimer <= 0f)
            StopSlide();
    }
}
