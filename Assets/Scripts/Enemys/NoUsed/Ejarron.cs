using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ejarron : EnemisBehaivor, Idamagable
{
    public float dmg; // Daño
    public float life; // Vida inicial
    public float attackCooldown; // Tiempo entre ataques
    public float attackRange; // Rango de ataque
    public float followSpeed; // Velocidad al perseguir

    private float lastAttackTime;
    private bool isAwake = false; // Controla si el mímico está activo

    [SerializeField] GameObject intactObject;
    [SerializeField] GameObject brokenObject;

    [Header("Sounds")]
    [SerializeField] private AudioClip jarronLaughClip;
    [SerializeField] private AudioClip jarronDamageClip;

    private void Awake()
    {
        currentlife = FlyweightPointer.Eshoot.maxLife + life;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = followSpeed;
            agent.isStopped = true; // El mímico no se mueve hasta que despierte
        }
    }

    private void Update()
    {
        if (isAwake && currentlife > 0)
        {
            AttackOrFollowPlayer();
        }
    }

    private void AttackOrFollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Persigue al jugador
            if (agent != null && agent.isStopped)
            {
                agent.isStopped = false; // Habilita el movimiento
            }
            agent.SetDestination(player.position);
        }
        else
        {
            // Detén al agente al estar dentro del rango de ataque
            if (agent != null && !agent.isStopped)
            {
                agent.isStopped = true;
            }

            // Ataca al jugador si el cooldown lo permite
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                player.GetComponent<Idamagable>().TakeDamage(dmg);
                lastAttackTime = Time.time;
                SoundManager.instance.PlaySound(jarronLaughClip, transform, 1f, false);
            }
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (!isAwake)
        {
            isAwake = true; // Despierta al mímico
            agent.isStopped = false; // Activa el movimiento
        }

        currentlife -= dmg;

        healParticle.SetActive(false);

        GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
        GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);

        Destroy(debris, 5);
        Destroy(acid, 15);

        if (currentlife <= 0)
        {
            Debug.Log("El mímico ha sido destruido.");
            GameManager.instance.enemys.Remove(this.gameObject);

            if (gameObject.tag == "Skeleton")
                SoundManager.instance.PlaySound(jarronDamageClip, transform, 1f, false);

            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }

            Break();
            Destroy(this.gameObject, 1f);
            PlayerHealth.instance.life += 10;
        }
    }

    private void Break()
    {
        intactObject.SetActive(false);
        brokenObject.SetActive(true);
    }
}