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
        // GeneratePatrolPoint();
        StartCoroutine(FOVRoutime());
    }

    private void Update()
    {
        if (currentlife <= 0) return;

        /* if (canSeePlayer)
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
         }*/
        firstNode = GameManager.instance.firstquestion;

        fsm = new FSM();
        fsm.CreateState("Attack", new AttackEnemy(fsm, this));
        fsm.CreateState("Escape", new Escape(fsm, this));
        fsm.CreateState("Walk", new Walk(fsm, this));
        fsm.ChangeState("Walk");
    }

    private void ChasePlayer()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        anim.SetBool("isMoving", true);
    }

    public override void AttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        agent.isStopped = true;

        if (attackRange < distanceToPlayer)
        {
            ChasePlayer();
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Atacando");
            resetAnim();
            anim.SetBool("Atack", true);
            player.GetComponent<Idamagable>().TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }

        Invoke(nameof(ResumeMovement), 1.5f);
    }

    private void ResumeMovement()
    {
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
    }

    public override void Patrol()
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
