using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Stats")]
    public float damage;

    [Header("References")]
    public Rigidbody rb;

    [Header("Variables")]
    private float counter;
    public float lifetime;

    [Header("Particle")]
    public GameObject wallHitEffect;
    public GameObject enemyHitEffect;

    void Update()
    {
        Lifetime();
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    public void Lifetime()
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
        if (other.CompareTag("Player"))
            return;

        if (other.CompareTag("Bullet"))
            return;

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<E_Shooter>().TakeDamage(damage);
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
            AudioManager.instance.PlaySFXOneShot("Bullet Enemy Impact", 1f);
        }
        if (other.CompareTag("Wall"))
            Instantiate(wallHitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}