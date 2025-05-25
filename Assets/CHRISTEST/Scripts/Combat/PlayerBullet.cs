using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Stats")]
    public float damage;

    [Header("References")]
    public Rigidbody rb;

    [Header("Lifetime")]
    public float lifetime = 5f;
    private float counter;

    [Header("Particles")]
    public GameObject wallHitEffect;
    public GameObject enemyHitEffect;

    void Update()
    {
        HandleLifetime();

        if (rb != null && rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    void HandleLifetime()
    {
        counter += Time.deltaTime;
        if (counter >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore self and player
        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
            return;

        // Enemy hit logic
        if (other.CompareTag("Enemy"))
        {
            E_Shooter enemy = other.GetComponent<E_Shooter>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            if (enemyHitEffect != null)
                Instantiate(enemyHitEffect, transform.position, Quaternion.identity);

            if (AudioManager.instance != null)
                AudioManager.instance.PlaySFXOneShot("Bullet Enemy Impact", 1f);

            HitmarkerController hitmarker = FindObjectOfType<HitmarkerController>();
            if (hitmarker != null)
                hitmarker.ShowHitmarker();
        }

        // Wall hit logic
        if (other.CompareTag("Wall"))
        {
            if (wallHitEffect != null)
                Instantiate(wallHitEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}