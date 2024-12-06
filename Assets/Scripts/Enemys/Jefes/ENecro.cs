using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ENecro : EnemisBehaivor
{
    public float necroLife;
    public float maxHealth;
    public Image healthBar;

    [Header("Fases de Combate")]
    public float phase2Threshold;
    public float phase3Threshold;
    private int currentPhase = 1;

    [Header("Invoker Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] summonPoints;
    public float summonCooldown;
    public int maxSummoned;
    private int currentEnemiesSummoned;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform shotPoint;
    public float shotCooldown;
    public float projectileSpeed;

    [Header("Ability Settings")]
    public GameObject laser;
    public GameObject shield;
    public float abilityCooldown;
    private float abilityTimer;

    [Header("Phase Effects")]
    public GameObject teleportEffect;
    public Transform[] teleportPoints;

    [Header("Shield Effect")]
    public ShieldShader _shieldShader;

    [Header("Misc Settings")]
    public float movementSpeed;

    [Header("Skull Summon Settings")]
    public GameObject skullPrefab;
    public int skullCount = 3;
    public float skullSpawnRadius = 2f;
    public float skullSpeed = 5f;

    [Header("Sounds")]
    [SerializeField] private AudioClip splatClip;
    [SerializeField] private AudioClip necroDeathClip;
    [SerializeField] private AudioClip necroGruntClip;
    [SerializeField] private AudioClip necroTeleportClip;
    [SerializeField] private AudioClip necroSummonClip;

    private float summonTimer;
    private float shotTimer;

    private void Awake()
    {
        summonTimer = summonCooldown;
        shotTimer = shotCooldown;
        abilityTimer = abilityCooldown;
        currentlife = necroLife;

        maxHealth = necroLife;
        healthBar.fillAmount = necroLife / maxHealth;
    }

    private void Start()
    {
        BossTrigger.instance.defeatText.gameObject.SetActive(false);
        phase2Threshold = necroLife / 2;
        phase3Threshold = necroLife / 3;
    }

    private void Update()
    {
        HandlePhases();
        EnemiMovement();
        healthBar.fillAmount = necroLife / maxHealth;
    }

    public void ResetAnim()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Summon", false);
        anim.SetBool("Atack", false);
        anim.SetBool("Fuego", false);
        anim.SetBool("Death", false);
    }

    private void HandlePhases()
    {
        if (currentlife <= phase3Threshold && currentPhase < 3)
        {
            EnterPhase3();
        }
        else if (currentlife <= phase2Threshold && currentPhase < 2)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        currentPhase = 2;
        Debug.Log("Entrando en Fase 2: Más habilidades defensivas y ofensivas.");
        // Aumentar la velocidad de ataque, reducir cooldowns
        TeleportRandomly();
        summonCooldown *= 0.8f;
        shotCooldown *= 0.8f;
    }

    private void EnterPhase3()
    {
        currentPhase = 3;
        Debug.Log("Entrando en Fase 3: Ataques devastadores.");
        // Activar invocaciones rápidas y ataques potentes
        summonCooldown *= 0.5f; 
        TeleportRandomly();
        abilityCooldown *= 0.5f;
    }

    public void EnemiMovement()
    {
        if (canSeePlayer)
        {
            summonTimer -= Time.deltaTime;
            shotTimer -= Time.deltaTime;
            abilityTimer -= Time.deltaTime;

            if (summonTimer <= 0)
            {
                SummonEnemy();
                summonTimer = summonCooldown;
            }

            if (shotTimer <= 0)
            {
                Shoot();
                shotTimer = shotCooldown;
            }

            if (abilityTimer <= 0)
            {
                UseRandomAbility();
                abilityTimer = abilityCooldown;
            }
            ResetAnim();
            anim.SetBool("Idle", true);
        }
        else
        {
            ResetAnim();
            anim.SetBool("Idle", true);
        }
    }

    private void SummonEnemy()
    {
        if (currentEnemiesSummoned >= maxSummoned) return;
        ResetAnim();
        anim.SetBool("Summon", true);

        Transform spawnPoint = summonPoints[Random.Range(0, summonPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint.position, Quaternion.identity);
        SoundManager.instance.PlaySound(necroSummonClip, transform, 1f, false);
        currentEnemiesSummoned++;
    }

    private void Shoot()
    {
        ResetAnim();
        anim.SetBool("Atack", true);
        SoundManager.instance.PlaySound(splatClip, transform, 1f, false);
        GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, Quaternion.identity);
        Vector3 directionToPlayer = (player.transform.position - shotPoint.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = directionToPlayer * projectileSpeed;
    }

    private void UseRandomAbility()
    {
        float randomValue = Random.value;
        if (randomValue < 0.33f)
        {
            ActivateShield();
        }
        else if (randomValue < 0.66f)
        {
            UseLaser();
        }
        else
        {
            UseSkullSummon();
        }
    }

    private void UseSkullSummon()
    {
        Debug.Log("Habilidad activada: Invocar Calaveras");

        for (int i = 0; i < skullCount; i++)
        {
            // Generar posición aleatoria alrededor del necromante
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * skullSpawnRadius;
            spawnPosition.y = transform.position.y; // Asegurar que estén al mismo nivel

            // Instanciar la calavera
            GameObject skull = Instantiate(skullPrefab, spawnPosition, Quaternion.identity);

            // Configurar el objetivo del jugador
            SkullBehavior skullBehavior = skull.GetComponent<SkullBehavior>();
            if (skullBehavior != null)
            {
                skullBehavior.SetTarget(player.gameObject);
                skullBehavior.speed = skullSpeed;
            }
        }
    }

    private void ActivateShield()
    {
        //shield.SetActive(true);
        
        if(_shieldShader._shieldOn == true)
        {
            _shieldShader._shieldCollider.enabled = true;
            _shieldShader.OpenCloseShield();
            _shieldShader._life = 100;
        }
        
        Debug.Log("Escudo activado");
    }

    private void UseLaser()
    {
        ResetAnim();
        anim.SetBool("Fuego", true);
        laser.SetActive(true);
        Debug.Log("Láser activado");
    }

    private void TeleportRandomly()
    {
        if (teleportPoints.Length == 0) return;

        Transform randomPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];
        // Instantiate(teleportEffect, transform.position, Quaternion.identity);
        SoundManager.instance.PlaySound(necroTeleportClip, transform, 1f, false);
        transform.position = randomPoint.position;
       // Instantiate(teleportEffect, transform.position, Quaternion.identity);

        Debug.Log("Teletransporte realizado");
    }

    public override void TakeDamage(float dmg)
    {
        necroLife -= dmg;
        healthBar.fillAmount = necroLife / maxHealth;
        SoundManager.instance.PlaySound(necroGruntClip, transform, 1f, false);

        if (currentlife <= 0)
        {
            BossTrigger.instance.bossHealth.gameObject.SetActive(false);
            anim.SetBool("Death", true);
            SoundManager.instance.PlaySound(necroDeathClip, transform, 1f, false);
            BossTrigger.instance.defeatText.gameObject.SetActive(true);
            Destroy(gameObject, 1);
        }
    }
}
