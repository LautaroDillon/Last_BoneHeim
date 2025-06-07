using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StomachGrenade : OrganGrenade
{
    public float dotDuration = 5f;
    public float dotDamagePerSecond = 5f;

    protected override void DoExplosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);

        foreach (var hit in hitColliders)
        {
            IDamagable damageable = hit.GetComponent<IDamagable>();
            if (damageable != null)
            {
                StartCoroutine(ApplyDOT(damageable));
            }
        }
    }

    private IEnumerator ApplyDOT(IDamagable target)
    {
        float elapsed = 0f;
        while (elapsed < dotDuration)
        {
            target.TakeDamage(dotDamagePerSecond);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }
}
