using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class E_Illusionist : EnemisBehaivor
{
    [Header("Patrol Settings")]
    public float patrolRadius;
    public float waitTime;
    private float waitTimer;
    private Vector3 patrolPoint;
    private bool isPatrolling;

    [Header("Attack")]
    public float attackRange;
    public float attackCooldown;
    private float attackTimer;
    public int damage;

    [Header("Illusions")]
    public GameObject illusionPrefab;
    public int numberOfIllusions;
    public float illusionDuration;
    public float illusionSpawnRadius;
    public float timeIllusions;

    [Header("Detection")]
    public float detectionRange;

    private bool isCreatingIllusions = false;

    [Header("Movement")]
    public NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Atack", false);
        anim.SetBool("Idle", false);

    }

    void Update()
    {
        if (currentlife <= 0)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (canSeePlayer)
        {
            if (distanceToPlayer > attackRange)
            {
                MoveToPlayer();
            }
            else
            {
                if (Time.time >= attackTimer + attackCooldown)
                {
                    AttackPlayer();
                }
            }

            if (!isCreatingIllusions && Random.Range(0, 100) < 10) // Probabilidad de crear ilusiones.
            {
                StartCoroutine(CreateIllusions());
            }
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (!isPatrolling)
        {
            waitTimer += Time.deltaTime;
            resetAnim();
            anim.SetBool("Idle", true);

            if (waitTimer >= waitTime)
            {
                GeneratePatrolPoint();
                waitTimer = 0;
            }
        }
        else
        {
            navMeshAgent.SetDestination(patrolPoint);
            resetAnim();
            anim.SetBool("Walk", true);

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                isPatrolling = false;
                navMeshAgent.isStopped = true;
            }
        }
    }

    private void GeneratePatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            navMeshAgent.isStopped = false;
            isPatrolling = true;
        }
    }

    private void MoveToPlayer()
    {
        if (navMeshAgent.enabled)
        {
            anim.SetBool("Walking", true);
            navMeshAgent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        attackTimer = Time.time;
        anim.SetBool("Attack", true);
        Debug.Log("Illusionist attacks!");

        // Detectar al jugador dentro del rango de ataque.
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    private IEnumerator CreateIllusions()
    {
        isCreatingIllusions = true;
        Debug.Log("Creating illusions...");

        for (int i = 0; i < numberOfIllusions; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * illusionSpawnRadius;
            randomOffset.y = 0; // Mantener las ilusiones en el suelo.
            Vector3 spawnPosition = transform.position + randomOffset;

            GameObject illusion = Instantiate(illusionPrefab, spawnPosition, Quaternion.identity);
            illusion.GetComponent<Illusion>().Initialize(illusionDuration, player);

            yield return new WaitForSeconds(timeIllusions); // Tiempo entre la creaci�n de cada ilusi�n.
        }

        yield return new WaitForSeconds(illusionDuration);
        isCreatingIllusions = false;
    }
}