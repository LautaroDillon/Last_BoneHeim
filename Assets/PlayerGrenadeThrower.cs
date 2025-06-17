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

    public void ThrowGrenadeInstant(GrenadeType type, float force)
    {
        GameObject prefab = GrenadeHandler.Instance.GetGrenadePrefab(type);
        if (prefab == null) return;

        GameObject grenade = ObjectPool.Instance.Get(prefab, throwPoint.position, Quaternion.identity);

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 throwDirection = playerCamera.transform.forward + playerCamera.transform.up * 0.1f;
            rb.velocity = throwDirection.normalized * force;
        }

        OrganGrenade organGrenade = grenade.GetComponent<OrganGrenade>();
        if (organGrenade != null)
        {
            organGrenade.call();
        }
    }
}
