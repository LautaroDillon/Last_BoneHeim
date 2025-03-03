using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBehavior : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    public float damage = 10f; 
    public GameObject explosionEffect;
    private float timeAlive = 0;
    public float lifeTime = 5f;

    public void SetTarget(GameObject player)
    {
        target = player.transform;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target == null) return;
        timeAlive += Time.deltaTime;

        float angle = timeAlive * 2f;
        Vector3 offset = new Vector3(Mathf.Sin(angle) * 2f, 0, Mathf.Cos(angle) * 2f);
        Vector3 moveDirection = (target.position - transform.position + offset).normalized;

        transform.position += moveDirection * speed * Time.deltaTime;
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
        }
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
    }
}