using UnityEngine;
using UnityEngine.AI;

public class EMelee : EnemisBehaivor
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

    [Header("shield")]
    public float lifeShield;
    float currentlifeShield;
    public bool hasshield = true;

    [Header("NavMesh")]
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentlifeShield = lifeShield;
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
        healParticle.SetActive(false);

        GameObject acid;

        if (true)
        {
            currentlife -= dmg;
            acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
            GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);

            Destroy(acid, 15);
            Destroy(debris, 5);
        }


        if (currentlife <= 0)
        {
            Debug.Log("the skeleton received damage ");
            GameManager.instance.enemys.Remove(this.gameObject);

            if (gameObject.tag == "Skeleton")
                SoundManager.instance.PlaySound(skeletonDeathClip, transform, 1f, false);

            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }

            Destroy(acid);
            Destroy(this.gameObject, 0.1f);
            PlayerHealth.instance.life += 10;
            Guns.instance.bulletsLeft += Random.Range(1, 3) + gun.killReward;

        }
    }
}
