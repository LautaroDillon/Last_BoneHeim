using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBehavior : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float damage = 10f; 
    public GameObject explosionEffect; 

    public void SetTarget(GameObject player)
    {
        target = player.transform;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}