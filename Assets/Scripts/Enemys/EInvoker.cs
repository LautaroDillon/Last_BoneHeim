using UnityEngine;
using UnityEngine.AI;

public class EInvoker : EnemisBehaivor
{
    [Header("Invoker Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] summonPoints;
    public float summonCooldown;
    public int maxSummoned;
    public bool isMiniboss;

    [Header("NavMesh Settings")]
    private NavMeshAgent agent;

    private float summonTimer;
    private int currentEnemiesSummoned;

    private float patrolWaitTime = 3f;
    private float patrolTimer = 0f;

    void Awake()
    {
        currentlife = FlyweightPointer.Ehealer.maxLife;
        speed = FlyweightPointer.Ehealer.speed;
        summonTimer = summonCooldown;

        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
        }
        else
        {
            Debug.LogWarning("NavMeshAgent component is missing!");
        }
    }

    private void Update()
    {
        if (currentlife > 0)
        {
            EnemiMovement();
        }
    }

    public void EnemiMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (!canSeePlayer)
        {
            Patrol();
        }
        else
        {
            if (!isMiniboss)
            {
                ChasePlayer();
            }

            // Cuenta regresiva para invocar enemigos
            summonTimer -= Time.deltaTime;

            // Invocar enemigos si hay oportunidad
            if (summonTimer <= 0 && currentEnemiesSummoned < maxSummoned)
            {
                SummonEnemy();
                summonTimer = summonCooldown;
            }
        }
    }



    private void Patrol()
    {
        if (agent != null && !agent.hasPath)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }

                patrolTimer = 0f;
            }
        }

        // Configura las animaciones en base al movimiento del agente
        anim.SetBool("idle", agent.velocity.magnitude < 0.1f);
        anim.SetBool("Moving", agent.velocity.magnitude > 0.1f);
    }

    private void ChasePlayer()
    {
        if (agent != null)
        {
            agent.SetDestination(player.position);
        }

        anim.SetBool("idle", false);
        anim.SetBool("Moving", true);
    }

    void SummonEnemy()
    {
        // Escoge un punto de invocación aleatorio
        Vector3 spawnPosition;

        if (summonPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, summonPoints.Length);
            spawnPosition = summonPoints[randomIndex].position;
        }
        else
        {
            // Si no hay puntos definidos, invoca cerca del invocador
            spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        }

        // Escoge aleatoriamente qué tipo de enemigo invocar
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyToSummon = enemyPrefabs[randomEnemyIndex];

        Instantiate(enemyToSummon, spawnPosition, Quaternion.identity);

        currentEnemiesSummoned++;
    }
}