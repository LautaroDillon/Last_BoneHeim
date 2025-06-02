using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    public float damage = 10f;
    public float lifetime = 0.5f; // destroy after a short time

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            // Optionally destroy immediately after hitting player:
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
