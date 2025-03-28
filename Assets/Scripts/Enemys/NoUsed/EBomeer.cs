using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class EBomeer : EnemisBehaivor
{
    #region variables

    [Header("EBomeer Settings")]
    [SerializeField] private float radius; // Radio de explosión
    [SerializeField] private int damage; // Daño de explosión
    [SerializeField] private GameObject explosion; // Prefab de la explosión
    [SerializeField] private float chaseRange = 15f; // Rango para perseguir al jugador

    [Header("Patrol Settings")]
    public float patrolRadius;
    public float waitTime;
    private float waitTimer;
    private Vector3 patrolPoint;
    private bool isPatrolling;
    #endregion

    private void Awake()
    {
        currentlife = FlyweightPointer.Eboomer.maxLife;
        speed = FlyweightPointer.Eboomer.speed;

        // Obtén el componente NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        //firstNode = GameManager.instance.firstquestion;


        fsm = new FSM();
        fsm.CreateState("Attack", new AttackEnemy(fsm, this));
        fsm.CreateState("Escape", new Escape(fsm, this));
        fsm.CreateState("Walk", new Walk(fsm, this));
        fsm.ChangeState("Walk");

        if (agent != null)
        {
            agent.speed = speed; // Configura la velocidad del NavMeshAgent
        }
    }

    private void Update()
    {
        if (currentlife <= 0)
        {
            return;
        }

        firstNode.Execute(this);
        fsm.Execute();
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
    public void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Atack", false);

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

    public override void AttackPlayer()
    {
        if (agent != null)
        {
            agent.isStopped = false; // Activa el movimiento
            agent.SetDestination(player.transform.position); // Persigue al jugador

            resetAnim();
            anim.SetBool("walking", true);
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