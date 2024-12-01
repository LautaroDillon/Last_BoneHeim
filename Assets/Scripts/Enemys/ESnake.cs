using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ESnake : EnemisBehaivor
{
    [Header("Attack Settings")]
    public float attackRange;
    public float attackCooldown;
    public float attackDamage;
    private float lastAttackTime;

    [Header("Patrol Settings")]
    public float patrolRadius;
    public float waitTime;
    private float waitTimer;
    private Vector3 patrolPoint;
    private bool isPatrolling;

    [Header("NavMesh")]
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        GeneratePatrolPoint();
    }

    private void Update()
    {
        if (currentlife <= 0) return;

        detection();

        if (canSeePlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void ChasePlayer()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        anim.SetBool("isMoving", true);
    }

    private void AttackPlayer()
    {
        navMeshAgent.isStopped = true;
        anim.SetBool("isMoving", false);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("Attack");
            Debug.Log("La serpiente ataca al jugador");
            player.GetComponent<Idamagable>().TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    private void Patrol()
    {
        if (!isPatrolling)
        {
            waitTimer += Time.deltaTime;
            anim.SetBool("isMoving", false);

            if (waitTimer >= waitTime)
            {
                GeneratePatrolPoint();
                waitTimer = 0;
            }
        }
        else
        {
            navMeshAgent.SetDestination(patrolPoint);
            anim.SetBool("isMoving", true);

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
    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        anim.SetTrigger("TakeDamage");
    }
}
