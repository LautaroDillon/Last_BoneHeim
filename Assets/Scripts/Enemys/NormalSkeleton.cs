using UnityEngine;
using UnityEngine.AI; 

public class NormalSkeleton : EnemisBehaivor
{
    [Header("NavMesh Settings")]
    private NavMeshAgent agent; 
    [SerializeField] private float stoppingDistance; // Distancia para detenerse cerca del jugador

    [Header("Attack Settings")]
    [SerializeField] private Transform firePoint; // Punto desde el cual dispara
    [SerializeField] private float shootRange; // Rango de disparo
    [SerializeField] private GameObject BulletPrefab; // Prefab de la bala
    [SerializeField] private float fireRate; // Velocidad de disparo
    [SerializeField] private float projectileSpeed; // Velocidad de la bala

    [Header("Movement Settings")]
    public bool isTurret; // Si el esqueleto es fijo
    private bool isAttacking;
    private float nextFireTime;

    private void Awake()
    {
        currentlife = FlyweightPointer.Eshoot.maxLife;
        agent = GetComponent<NavMeshAgent>(); // Obtiene el NavMeshAgent
        instance = this;

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance; // Define la distancia mínima para detenerse
        }
    }

    private void Update()
    {
        if (currentlife > 0)
        {
            EnemiMovement();
        }
    }

    private void EnemiMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (!canSeePlayer) // Si no ve al jugador
        {
            Patrol();
        }
        else
        {
            if (distanceToPlayer > shootRange && !isAttacking && !isTurret)
            {
                ChasePlayer();
            }
            else if (distanceToPlayer <= shootRange)
            {
                AttackPlayer();
            }
        }
    }

    private void Patrol()
    {
        // Patrulla aleatoria
        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            cronometro += Time.deltaTime;
            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    // No hace nada (idle)
                    break;
                case 1:
                    // Se mueve hacia una dirección aleatoria
                    Vector3 randomDirection = Random.insideUnitSphere * 5f;
                    randomDirection += transform.position;
                    if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }
                    break;
            }
        }
    }

    private void ChasePlayer()
    {
        if (agent != null)
        {
            agent.isStopped = false; // Permite el movimiento
            agent.SetDestination(player.transform.position); // Persigue al jugador
        }
    }

    private void AttackPlayer()
    {
        if (agent != null)
        {
            agent.isStopped = true; // Detiene el movimiento mientras ataca
        }

        // Gira hacia el jugador
        Vector3 lookPos = player.transform.position - transform.position;
        lookPos.y = 0; // Ignora el eje Y
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 5 * Time.deltaTime);

        // Ataca si puede disparar
        if (CanFire())
        {
            Shoot();
        }

        isAttacking = true;
        EndAttack();
    }

    private bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    private void Shoot()
    {
        // Calcula la dirección hacia el jugador
        Vector3 directionToPlayer = (player.transform.position - firePoint.position).normalized;

        // Obtiene una bala del Bullet Manager
        var bullet = BuletManager.instance.GetBullet();
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.forward = directionToPlayer;

        // Añade velocidad a la bala
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * projectileSpeed;
        }

        nextFireTime = Time.time + 1f / fireRate;
    }

    private void EndAttack()
    {
        isAttacking = false;
    }
}