using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ENecro : EnemisBehaivor
{
    #region Variables
    public float maxHealth;
    public Image healthBar;

    public static ENecro instanse;

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

    [SerializeField]  private float summonTimer;
    [SerializeField]  private float shotTimer;
    #endregion

    #region basics
    private void Awake()
    {
        summonTimer = summonCooldown;
        shotTimer = shotCooldown;
        abilityTimer = abilityCooldown;
        instanse = this;
        maxHealth = necroLife;
        healthBar.fillAmount = necroLife / maxHealth;
        StartCoroutine(FOVRoutime());

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

        if (canSeePlayer && player != null) // Solo mira si detecta al jugador
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }

    }
    #endregion

    public void ResetAnim()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Summon", false);
        anim.SetBool("Atack", false);
        anim.SetBool("Fuego", false);
        anim.SetBool("Death", false);
    }

    #region phases
    private void HandlePhases()
    {
        if (necroLife <= phase3Threshold && currentPhase < 3)
        {
            EnterPhase3();
        }
        else if (necroLife <= phase2Threshold && currentPhase < 2)
        {
            EnterPhase2();
        }
    }

    private void EnterPhase2()
    {
        currentPhase = 2;
        Debug.Log("Fase 2: Invoca m�s y ataca m�s r�pido.");
        summonCooldown *= 0.8f;
        shotCooldown *= 0.8f;
        skullSpeed += 2f; // Calaveras m�s r�pidas
       // movementSpeed *= 1.2f; // Se mueve m�s r�pido
        TeleportRandomly();
    }

    private void EnterPhase3()
    {
        currentPhase = 3;
        Debug.Log("Fase 3: Se vuelve agresivo y teletransporta m�s.");
        summonCooldown *= 0.7f;  // En lugar de 0.5f
        shotCooldown *= 0.8f;    // No dispara tan r�pido
        abilityCooldown *= 0.7f;
        // movementSpeed *= 1.5f;
        StartCoroutine(Phase3Aggression());
        TeleportRandomly();
    }

    private IEnumerator Phase3Aggression()
    {
        while (currentPhase == 3 && necroLife > 0)
        {
            UseRandomAbility();
            yield return new WaitForSeconds(abilityCooldown / 2);
        }
    }
    #endregion

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
                shotTimer = shotCooldown;
                Shoot();
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

    #region Ability Methods
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
        if (necroLife > maxHealth * 0.2f)  // Solo activa escudo si est� por morir
        {
            if (randomValue < 0.4f)
            {
                UseLaser();
            }
            else
            {
                UseSkullSummon();
            }
        }
        else
        {
            ActivateShield();  // Solo se activa en momentos cr�ticos
        }
    }

    private void UseSkullSummon()
    {
       // Debug.Log("Habilidad activada: Invocar Calaveras");

        for (int i = 0; i < skullCount; i++)
        {
            // Generar posici�n aleatoria alrededor del necromante
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * skullSpawnRadius;
            spawnPosition.y = transform.position.y; // Asegurar que est�n al mismo nivel

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
        laser.GetComponent<Lazer>().timer = 5;
        laser.SetActive(true);
        Debug.Log("L�ser activado");
    }

    private void TeleportRandomly()
    {
        if (teleportPoints.Length == 0) return;

        Transform randomPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];
        Instantiate(teleportEffect, transform.position, Quaternion.identity);
        SoundManager.instance.PlaySound(necroTeleportClip, transform, 1f, false);
        transform.position = randomPoint.position;

        Debug.Log("Teletransporte realizado");
    }
    #endregion

    public override void TakeDamage(float dmg)
    {
        necroLife -= dmg;
        healthBar.fillAmount = necroLife / maxHealth;
        SoundManager.instance.PlaySound(necroGruntClip, transform, 1f, false);

        if (necroLife <= 0)
        {
            BossTrigger.instance.bossHealth.gameObject.SetActive(false);
            ResetAnim();
            anim.SetBool("Death", true);
            SoundManager.instance.PlaySound(necroDeathClip, transform, 1f, false);
            BossTrigger.instance.defeatText.gameObject.SetActive(true);
            Destroy(gameObject, 1);
        }
    }
}
