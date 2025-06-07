using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartGrenade : OrganGrenade
{
    protected override void DoExplosion()
    {
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
    }
}
