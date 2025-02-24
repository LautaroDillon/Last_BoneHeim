using UnityEngine;
using UnityEngine.AI;

public class EInvoker : EnemisBehaivor
{
    #region variables
    [Header("Invoker Settings")]
    public GameObject[] enemyPrefabs;
    public Transform[] summonPoints;
    public float summonCooldown;
    public int maxSummoned;
    public bool isMiniboss;

    private float summonTimer;
    private int currentEnemiesSummoned;

    [Header("Patrol Settings")]
    public float patrolRadius;
    public float waitTime;
    private float waitTimer;
    private Vector3 patrolPoint;
    private bool isPatrolling;
#endregion

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
                EscapePlayer();
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
    public void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Atack", false);

    }

    private Vector3 GenerateRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return transform.position;
    }

    private void Patrol()
    {
        // Si no está patrullando, genera un nuevo punto de patrulla
        if (!isPatrolling)
        {
            patrolPoint = GenerateRandomPatrolPoint();
            agent.SetDestination(patrolPoint);
            agent.isStopped = false;
            isPatrolling = true;
            resetAnim();
            anim.SetBool("Walk", true);
        }

        // Si llega al destino, inicia el temporizador de espera
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true; // Detiene el movimiento
            resetAnim();
            anim.SetBool("Idle", false);

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                isPatrolling = false;
                waitTimer = 0f;
                agent.isStopped = false;
            }
        }
    }

    private void EscapePlayer()
    {
        if (agent != null)
        {
            // Calcula la dirección opuesta al jugador
            Vector3 directionAwayFromPlayer = transform.position - player.position;
            directionAwayFromPlayer.Normalize();

            // Define un punto de destino alejado del jugador
            Vector3 escapeDestination = transform.position + directionAwayFromPlayer * 10f;

            // Verifica si el punto es válido dentro del NavMesh
            if (NavMesh.SamplePosition(escapeDestination, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                // Si no se encuentra una posición válida, elige una dirección aleatoria
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit randomHit, 10f, NavMesh.AllAreas))
                {
                    agent.SetDestination(randomHit.position);
                }
            }
        }

        resetAnim();
        anim.SetBool("Moving", true);

        // Verifica si el agente está atascado
        if (agent.velocity.sqrMagnitude < 0.01f && !agent.pathPending)
        {
            // Recalcula la ruta si no está avanzando
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit randomHit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(randomHit.position);
            }
        }
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