using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENecro : EnemisBehaivor
{
    [Header("Invoker Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] summonPoints;
    public float summonCooldown;
    public int maxSummoned;
    public bool isMiniBoss;

    [Header("summon Settings")]
    public float summonTimer;
    int currentEnemiesSummoned;
    public bool cansummon;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform shotPoint;
    public float shotCooldown;
    public float projectileSpeed;
    public float shotTimer;

    [Header("abilitys Settings")]
    //public GameObject lazer;
   // public GameObject coolDown;
    public GameObject shield;
    public float countDown;
   // public float probabilityLaser;
    public float probabilityShield;
    public bool canability;


    void Awake()
    {
        summonTimer = summonCooldown;
        shotTimer = shotCooldown;
    }

    private void Update()
    {
        EnemiMovement();
    }

    public void EnemiMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= ranged)
        {

            summonTimer -= Time.deltaTime;
            shotTimer -= Time.deltaTime;
            countDown -= Time.deltaTime;

            if (summonTimer <= 0 && cansummon)
            {
                SummonEnemy();
                summonTimer = summonCooldown;
            }

            if (shotTimer <= 0)
            {
                Shoot();
                shotTimer = shotCooldown;
            }

            if (countDown <= 0 && canability)
            {
                float numeroAleatorio = Random.Range(0f, 11f);
               /* if (probabilityLaser < numeroAleatorio)
                {
                   // coolDown.SetActive(true);
                }*/
                if(probabilityShield < numeroAleatorio /*&& probabilityLaser > numeroAleatorio*/)
                {
                    shield.SetActive(true);
                }
            }
        }
    }

    void SummonEnemy()
    {
        Vector3 spawnPosition;
        if (summonPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, summonPoints.Length);
            spawnPosition = summonPoints[randomIndex].position;
        }
        else
        {
            // Si no hay puntos de invocación, invoca cerca del enemigo
            spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        }

        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSummon = enemyPrefabs[randomEnemyIndex];
        Instantiate(enemyToSummon, spawnPosition, Quaternion.identity);

        currentEnemiesSummoned++;
    }

    void Shoot()
    {
        if (projectilePrefab != null && shotPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, Quaternion.identity);

            Vector3 directionToPlayer = (player.transform.position - shotPoint.position).normalized;

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * projectileSpeed;
            }
        }
    }
}
