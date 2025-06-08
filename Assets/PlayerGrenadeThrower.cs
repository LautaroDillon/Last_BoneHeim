using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerGrenadeThrower : MonoBehaviour
{
    [Header("Throw Settings")]
    public Transform throwPoint;
    public float minThrowForce = 5f;
    public float maxThrowForce = 20f;
    public float maxChargeTime = 2f;
    public Camera playerCamera;

    [Header("Grenade Type")]
    public GrenadeType currentGrenadeType = GrenadeType.Heart;

    private float currentCharge = 0f;
    private bool isCharging = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentGrenadeType = GrenadeType.Heart;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentGrenadeType = GrenadeType.Lung;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentGrenadeType = GrenadeType.Stomach;

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
    }

    void ChargeThrow()
    {
        if (!isCharging) return;

        currentCharge += Time.deltaTime;
        currentCharge = Mathf.Clamp(currentCharge, 0f, maxChargeTime);
    }

    void ThrowGrenade()
    {
        if (!isCharging) return;

        isCharging = false;

        float normalizedCharge = currentCharge / maxChargeTime;
        float finalThrowForce = Mathf.Lerp(minThrowForce, maxThrowForce, normalizedCharge);
        currentCharge = 0f;

        // Shake camera
        CameraShake.Instance.ShakeOnce(2f, 0.1f, 1f, 1f);

        GameObject prefab = GrenadeHandler.Instance.GetGrenadePrefab(currentGrenadeType);
        if (prefab == null) return;

        GameObject grenade = ObjectPool.Instance.Get(prefab, throwPoint.position, Quaternion.identity);

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwDirection = playerCamera.transform.forward + playerCamera.transform.up * 0.1f;
            rb.velocity = throwDirection.normalized * finalThrowForce;
        }
    }
}
