using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganGrenade : MonoBehaviour
{
    [Header("Settings")]
    public float delay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float damage = 50f;
    public LayerMask damageMask;

    [Header("Effects")]
    public GameObject explosionVFX;
    public AudioClip explosionSFX;

    private bool hasExploded = false;

    private void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionVFX)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        if (explosionSFX)
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);

        foreach (var hit in hitColliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
                damageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
