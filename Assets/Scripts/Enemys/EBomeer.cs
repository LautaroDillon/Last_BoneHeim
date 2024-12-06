using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class EBomeer : EnemisBehaivor
{
    [Header("NavMesh Settings")]
    private NavMeshAgent agent;

    [Header("EBomeer Settings")]
    [SerializeField] private float radius; // Radio de explosión
    [SerializeField] private int damage; // Daño de explosión
    [SerializeField] private GameObject explosion; // Prefab de la explosión
    [SerializeField] private float chaseRange = 15f; // Rango para perseguir al jugador

    private void Awake()
    {
        currentlife = FlyweightPointer.Eboomer.maxLife;
        speed = FlyweightPointer.Eboomer.speed;

        // Obtén el componente NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = speed; // Configura la velocidad del NavMeshAgent
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

        if (distanceToPlayer > chaseRange)
        {
            Patrol(); // Patrulla cuando está lejos del jugador
        }
        else
        {
            ChaseAndExplode(); // Persigue al jugador y explota al estar cerca
        }
    }

    private void Patrol()
    {
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
                    anim.SetBool("idle", true);
                    anim.SetBool("walking", false);
                    break;
                case 1:
                    // Mueve hacia una posición aleatoria en el NavMesh
                    Vector3 randomDirection = Random.insideUnitSphere * 5f;
                    randomDirection += transform.position;

                    if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                    {
                        agent.SetDestination(hit.position);
                    }

                    anim.SetBool("idle", false);
                    anim.SetBool("walking", true);
                    break;
            }
        }
    }

    private void ChaseAndExplode()
    {
        if (agent != null)
        {
            agent.isStopped = false; // Activa el movimiento
            agent.SetDestination(player.transform.position); // Persigue al jugador

            anim.SetBool("walking", true);
            anim.SetBool("idle", false);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= agent.stoppingDistance)
        {
            StartCoroutine(Explosion(0.5f)); // Explota al estar suficientemente cerca
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1);
            StartCoroutine(Explosion(1.5f)); // Explota tras un breve retraso al recibir daño
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            TakeDamage(1);
            StartCoroutine(Explosion(1.5f)); // Explota tras un breve retraso al colisionar
        }
    }

    public IEnumerator Explosion(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Elimina al enemigo y genera la explosión
        GameManager.instance.enemys.Remove(this.gameObject);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.1f);
    }

    public override void TakeDamage(float dmg)
    {
        currentlife -= dmg;

        GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
        Destroy(acid, 3);

        if (currentlife <= 0)
        {
            if(Ally.intance != null)
            Ally.intance._enemies.Remove(this.gameObject);
            Destroy(acid);
            Destroy(this.gameObject, 0.1f);
        }
    }
}