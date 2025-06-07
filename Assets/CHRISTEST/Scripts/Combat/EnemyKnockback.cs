using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    private float pendingWallDamage = 0f;
    private bool canTakeWallDamage = false;

    public LayerMask wallMask;

    public void SetPendingWallDamage(float damage)
    {
        pendingWallDamage = damage;
        canTakeWallDamage = false;
        Invoke(nameof(EnableWallDamage), 0.1f); // delay to avoid self-collision
    }

    private void EnableWallDamage()
    {
        canTakeWallDamage = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canTakeWallDamage || pendingWallDamage <= 0f) return;

        if (((1 << collision.gameObject.layer) & wallMask.value) != 0)
        {
            IDamagable damageable = GetComponent<IDamagable>();
            if (damageable != null)
            {
                damageable.TakeDamage(pendingWallDamage);
            }

            pendingWallDamage = 0f;
            canTakeWallDamage = false;
        }
    }
}
