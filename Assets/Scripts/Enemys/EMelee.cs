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
    GameObject ShieldOBJ;
    public float lifeShield;
    float currentlifeShield;
    public bool hasshield = true;

    [Header("NavMesh")]
    private NavMeshAgent navMeshAgent;

    GameObject acid;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        currentlifeShield = lifeShield;
        hasshield = true;
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

    public void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Atack", false);
        anim.SetBool("Idle", false);

    }

    private void ChasePlayer()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        resetAnim();
        anim.SetBool("Walk", true);
    }

    private void AttackPlayer()
    {
        navMeshAgent.isStopped = true;
        resetAnim();

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetBool("Atack", true);
            player.GetComponent<Idamagable>().TakeDamage(attackDamage);
            lastAttackTime = Time.time;
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

    public override void TakeDamage(float dmg)
    {
        healParticle.SetActive(false);

        if (hasshield)
        {
            currentlife -= dmg;
            acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
            GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);

            Destroy(acid, 15);
            Destroy(debris, 5);
        }
        else
        {
            currentlifeShield -= dmg;
            ShieldOBJ.SetActive(false);
        }

        if (currentlifeShield <= 0)
        {
            hasshield = false;
        }


        if (currentlife <= 0)
        {
            resetAnim();

            GameManager.instance.enemys.Remove(this.gameObject);

            if (gameObject.tag == "Skeleton")
                SoundManager.instance.PlaySound(skeletonDeathClip, transform, 1f, false);

            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }

            Destroy(acid);
            anim.SetBool("Death", true);
            Destroy(this.gameObject, 2f);
            PlayerHealth.instance.life += 10;
            Guns.instance.bulletsLeft += Random.Range(1, 3) + gun.killReward;

        }
    }
}
