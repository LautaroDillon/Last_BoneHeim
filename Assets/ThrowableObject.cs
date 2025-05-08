using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public int damage = 20;
    public LayerMask damageableLayer;
    private bool hasHit = false;

    [Header("Particle")]
    public GameObject enemyHitEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
            return;

        if (((1 << collision.gameObject.layer) & damageableLayer) != 0)
        {
            hasHit = true;

            if (collision.gameObject.TryGetComponent(out IDamagable damageable))
            {
                damageable.TakeDamage(damage);
                Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                Debug.Log("Throwable hit: " + collision.gameObject.name);
            }

            if (collision.rigidbody != null)
            {
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                collision.rigidbody.AddForce(knockbackDirection * 5f, ForceMode.Impulse);
            }
        }
    }
}
