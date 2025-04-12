using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform cameraHolder;
    CapsuleCollider cc;
    private Rigidbody rb;
    private PlayerMovement pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

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
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 camPos = cameraHolder.localPosition;
        float targetY = pm.sliding ? slideCamYPos : startCamYPos;
        camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * 10f);
        cameraHolder.localPosition = camPos;

        if(pm.grounded)
        {
            if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
                StartSlide();

            if (Input.GetKeyUp(slideKey) && pm.sliding)
                StopSlide();
        }
        else
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        pm.sliding = true;
        AudioManager.instance.PlaySFXOneShot("Slide", 1f);
        cc.height = slideYScale;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }
    private void StopSlide()
    {
        pm.sliding = false;
        cc.height = startYScale;
        AudioManager.instance.StopSFX("Slide");
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // sliding normal
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        // sliding down a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }
}
