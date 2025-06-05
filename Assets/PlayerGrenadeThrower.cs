using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerGrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab;
    public Transform throwPoint;
    public float minThrowForce = 5f;
    public float maxThrowForce = 20f;
    public float maxChargeTime = 2f;
    public Camera playerCamera;

    private float currentCharge = 0f;
    private bool isCharging = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCharging();
        }

        if (Input.GetKey(KeyCode.G))
        {
            ChargeThrow();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            ThrowGrenade();
        }
    }

    void StartCharging()
    {
        isCharging = true;
        currentCharge = 0f;

        // Start camera shake
        CameraShake.Instance.StartShake(2f, 0.1f, 2f); // (magnitude, roughness, fadeInTime)
    }

    void ChargeThrow()
    {
        if (isCharging)
        {
            currentCharge += Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxChargeTime);
        }
    }

    void ThrowGrenade()
    {
        if (!isCharging)
            return;

        isCharging = false;

        // Stop camera shake
        //CameraShake.Instance.StopAllShakes();

        float normalizedCharge = currentCharge / maxChargeTime;
        float finalThrowForce = Mathf.Lerp(minThrowForce, maxThrowForce, normalizedCharge);

        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        Vector3 throwDirection = playerCamera.transform.forward + playerCamera.transform.up * 0.1f;
        rb.velocity = throwDirection.normalized * finalThrowForce;

        currentCharge = 0f;
    }
}
