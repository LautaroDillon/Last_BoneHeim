using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerProyectiles : MonoBehaviour
{
    public static PlayerProyectiles instance;

    [Header("Variables")]
    [SerializeField] private LayerMask enemyLayer;
    public float lifeTime = 2f;
    public float homingSpeed = 5f;
    private Transform targetEnemy;

    [Header("Sounds")]
    [SerializeField] private AudioClip bulletWallClip;
    [SerializeField] private AudioClip bulletHitClip;

    private void Awake()
    {
        Destroy(this.gameObject, lifeTime);
    }

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Guns.instance.isParasite == true)
        {
            Debug.Log("Homing Active!");
            FindNearestEnemy();
        }
        if (Guns.instance.isParasite && targetEnemy != null)
        {
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            transform.position += direction * homingSpeed * Time.deltaTime;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, homingSpeed * Time.deltaTime);
        }
    }

    private void FindNearestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        // Use Physics.OverlapSphere to get colliders in a certain radius on the enemy layer
        Collider[] colliders = Physics.OverlapSphere(transform.position, 300f, enemyLayer); // Adjust radius (100f) as needed

        foreach (Collider collider in colliders)
        {
            // Ensure the object has an IDamagable interface and check if it's an enemy
            Idamagable damagableInterface = collider.gameObject.GetComponent<Idamagable>();
            if (damagableInterface != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = collider.transform;
                }
            }
        }

        // Set the closest enemy as the target
        targetEnemy = closestEnemy;
    }

    public void ApplyLightningEffect(EnemisBehaivor hitEnemy, float lightningDamage, float radius, float chainChance, EnemisBehaivor[] allEnemies)
    {
        lightningDamage = Guns.instance.lightningDamage;
        chainChance = Guns.instance.lightningChance;
        hitEnemy.TakeDamage(lightningDamage);

        foreach (var enemy in allEnemies)
        {
            // Skip the hit enemy itself
            if (enemy != hitEnemy && IsInRange(hitEnemy, enemy, radius))
            {
                float chance = UnityEngine.Random.Range(1, 101);
                if (chance <= chainChance) // Chance to hit nearby enemy
                {
                    enemy.TakeDamage(lightningDamage);
                }
            }
        }
    }

    // Function to check if two enemies are within a certain radius
    private bool IsInRange(EnemisBehaivor firstEnemy, EnemisBehaivor secondEnemy, float radius)
    {
        float distance = (float)Math.Sqrt(Math.Pow(firstEnemy.transform.position.x - secondEnemy.transform.position.x, 2) + Math.Pow(firstEnemy.transform.position.y - secondEnemy.transform.position.y, 2));
        return distance <= radius;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12 && other.gameObject != this.gameObject && other.gameObject.tag == "Organ" && other.gameObject.tag == "Hand")
        {
            Physics.IgnoreCollision(other, this.GetComponent<Collider>());
            return;
        }

        if (other.gameObject.layer == 10)
        {
            Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();

            if (damagableInterface != null)
            {
                Debug.Log("Enemy Hit!");

                damagableInterface.TakeDamage(FlyweightPointer.Player.Damage);
                SoundManager.instance.PlaySound(bulletHitClip, transform, 0.3f, false);
                PlayerHealth.instance.life += PlayerHealth.instance.lifeSteal;
                if (!Guns.instance.isNail)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            SoundManager.instance.PlaySound(bulletWallClip, transform, 0.3f, false);
            Destroy(this.gameObject);
        }
    }
}
