using UnityEngine;
using UnityEngine.AI;

public class EHealer : EnemisBehaivor, Idamagable
{
    [Header("Healer")]
    public float attackRange = 1f;
    public GameObject healerBuff;
    public bool canspawn = true;

    private NavMeshAgent agent;
    private Vector3 wanderTarget;

    void Awake()
    {
        currentlife = FlyweightPointer.Ehealer.maxLife;
        speed = FlyweightPointer.Ehealer.speed;

        // Configurar NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
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

        if (distanceToPlayer > 20)
        {
            Wander(); // Movimiento aleatorio si está lejos del jugador.
        }
        else
        {
            FleeFromPlayer(); // Huir del jugador si está cerca.
        }
    }

    private void Wander()
    {
        if (!agent.hasPath || agent.remainingDistance < 1f)
        {
            wanderTarget = GetRandomWanderPosition();
            agent.SetDestination(wanderTarget);
        }

        anim.SetBool("Moving", true);
        anim.SetBool("idle", false);
    }

    private Vector3 GetRandomWanderPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // Generar un punto aleatorio.
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position; // En caso de no encontrar un punto válido, quedarse en el lugar.
    }

    private void FleeFromPlayer()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 fleePosition = transform.position + directionAwayFromPlayer.normalized * 10f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        anim.SetBool("Moving", true);
        anim.SetBool("idle", false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(1);
            Heal();
        }
    }

    void Heal()
    {
        if (currentlife <= 50 && canspawn)
        {
            GameObject healInstance = Instantiate(healerBuff, transform.position, Quaternion.identity);
            healInstance.GetComponent<Healing>().SetHealer(this);
            canspawn = false;
        }
    }
}