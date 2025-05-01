using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNoise : MonoBehaviour
{
    [Header("References")]
    public Transform cameraHolder;
    public Rigidbody playerRb;
    public PlayerMovement pm;

    [Header("Movement Noise")]
    public float moveNoiseIntensity = 0.05f;
    public float moveNoiseFrequency = 1.5f;

    [Header("Strafe Tilt")]
    public float maxTilt = 5f;
    public float tiltSpeed = 5f;

    [Header("Jump Bump")]
    public float jumpShakeAmount = 0.1f;
    public float bumpReturnSpeed = 5f;

    [Header("Landing Bump")]
    public float landingBumpAmount = -0.08f;
    public float landingBumpReturnSpeed = 8f;

    private float landingBump = 0f;
    private bool wasGroundedLastFrame = true;
    private float previousYVelocity;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private float noiseTime;

    private float currentJumpShake = 0f;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }

    void Update()
    {
        Vector3 localOffset = Vector3.zero;

        float speed = new Vector2(playerRb.velocity.x, playerRb.velocity.z).magnitude;

        //Move Noise
        if (speed > 0.1f && pm.grounded)
        {
            noiseTime += Time.deltaTime * moveNoiseFrequency;
            float offsetX = (Mathf.PerlinNoise(noiseTime, 0f) - 0.5f) * 2f;
            float offsetY = (Mathf.PerlinNoise(0f, noiseTime) - 0.5f) * 2f;

            localOffset += new Vector3(offsetX, offsetY, 0f) * moveNoiseIntensity;
        }

        // === Detect Landing ===
        bool justLanded = !wasGroundedLastFrame && pm.grounded;
        wasGroundedLastFrame = pm.grounded;

        if (justLanded)
        {
            float fallImpact = Mathf.Abs(previousYVelocity);

            // Clamp to avoid crazy shakes
            float impactScale = Mathf.Clamp01(fallImpact / 10f); // 10 = max velocity to care about
            landingBump = Mathf.Lerp(0f, landingBumpAmount, impactScale); // More fall = more bump
        }

        //Head Tilt
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetTilt = -horizontalInput * maxTilt;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetTilt);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, Time.deltaTime * tiltSpeed);

        //Jump Shake
        if (pm.state == PlayerMovement.MovementState.air && currentJumpShake <= 0f)
        {
            currentJumpShake = jumpShakeAmount;
        }
        currentJumpShake = Mathf.Lerp(currentJumpShake, 0f, Time.deltaTime * bumpReturnSpeed);

        landingBump = Mathf.Lerp(landingBump, 0f, Time.deltaTime * landingBumpReturnSpeed);

        Vector3 finalOffset = localOffset + new Vector3(0f, currentJumpShake + landingBump, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos + finalOffset, Time.deltaTime * 10f);
        previousYVelocity = playerRb.velocity.y;
    }

    public void TriggerJumpShake()
    {
        currentJumpShake = jumpShakeAmount;
    }
}
