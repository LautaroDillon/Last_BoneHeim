using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungGrenade : OrganGrenade
{
    [Header("Air Grenade Settings")]
    public float wallImpactDamage = 30f;

    protected override void DoExplosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);

        foreach (var hit in hitColliders)
        {
            Debug.Log("Lung Grenade hit: " + hit.name);
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }

            EnemyKnockback receiver = hit.GetComponent<EnemyKnockback>();
            if (receiver != null)
            {
                receiver.SetPendingWallDamage(wallImpactDamage);
            }
        }
    }
}
