using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class E_Illusionist : EnemisBehaivor
{
    #region variables
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
    public float patrolWaitTime = 3f;
    private float patrolTimer = 0f;

    float distanceToPlayer;
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameManager.instance.thisIsPlayer;
        StartCoroutine(FOVRoutime());
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        fsm = new FSM();
        fsm.CreateState("Attack", new AttackEnemy(fsm, this));
        fsm.CreateState("Escape", new Escape(fsm, this));
        fsm.CreateState("Walk", new Walk(fsm, this));
        fsm.ChangeState("Walk");
    }

    public override void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("idle", false);

    }

    void Update()
    {
        if (currentlife <= 0)
            return;

        /* float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
             resetAnim();
             anim.SetBool("idle", true);

         }*/

        firstNode.Execute(this);
        fsm.Execute();
    }

    /* private void RotateTowardsMovement()
     {
         if (navMeshAgent.velocity.sqrMagnitude > 0.1f) // Si se está moviendo
         {
             Vector3 direction = navMeshAgent.velocity.normalized;
             direction.y = 0; // Asegurarse de no cambiar la inclinación vertical
             transform.rotation = Quaternion.LookRotation(direction);
         }
     }*/

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

    public override void Patrol()
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
            anim.SetBool("Walk", false);   // Cambia la animación a idle o similar

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime) // Si el temporizador supera el tiempo de espera
            {
                isPatrolling = false;  // Permite generar un nuevo destino
                waitTimer = 0f;        // Reinicia el temporizador
                agent.isStopped = false;
            }
        }
    }

    private void MoveToPlayer()
    {
        if (agent.enabled)
        {
            resetAnim();

            anim.SetBool("walk", true);
            agent.SetDestination(player.position);
        }
    }

    public override void Escape()
    {
        MoveToHealer();
    }

    public override void AttackPlayer()
    {
        if (distanceToPlayer > attackRange)
        {
            MoveToPlayer();
        }

        if (!isCreatingIllusions && Random.Range(0, 100) < 10) // Probabilidad de crear ilusiones.
        {
            StartCoroutine(CreateIllusions());
        }

        if (distanceToPlayer > attackRange)
        {
            attackTimer = Time.time;
            // anim.SetBool("Attack", true);
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

            yield return new WaitForSeconds(timeIllusions); // Tiempo entre la creación de cada ilusión.
        }

        yield return new WaitForSeconds(illusionDuration);
        isCreatingIllusions = false;
    }
}
