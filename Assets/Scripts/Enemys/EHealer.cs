using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EHealer : EnemisBehaivor, Idamagable
{
    #region variables
    private Vector3 wanderTarget;

    public float patrolWaitTime = 3f;
    private float patrolTimer = 0f;

    [Header("Patrol Settings")]
    public float patrolRadius;
    public float waitTime;
    private float waitTimer;
    private Vector3 patrolPoint;
    private bool isPatrolling;

    [Header("Healerpower")]
    public float healRadius = 10.0f;
    public int healingAmount = 5;
    public float healInterval = 0.5f;
    public float healDuration = 4f;
    private bool isHealing = false;
    public int nearallies;
    #endregion

    void Awake()
    {
        enemyType = EnemyType.Healer;
        currentlife = FlyweightPointer.Ehealer.maxLife;
        speed = FlyweightPointer.Ehealer.speed;

        // Configurar NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        firstNode = GameManager.instance.firstquestion;

        GameManager.instance.Healers.Add(this);

        fsm = new FSM();
        fsm.CreateState("Attack", new AttackEnemy(fsm, this));
        fsm.CreateState("Escape", new Escape(fsm, this));
        fsm.CreateState("Walk", new Walk(fsm, this));
        fsm.CreateState("Heal", new Heal(fsm, this));
        fsm.ChangeState("Walk");

    }

    private void Update()
    {
        firstNode.Execute(this);
        fsm.Execute();
    }

    public void EnemiMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > 20)
        {
            Patrol(); // Movimiento aleatorio si está lejos del jugador.
        }
        else
        {
            FleeFromPlayer(); // Huir del jugador si está cerca.
        }
    }

    public override void AttackPlayer()
    {
        FleeFromPlayer();
    }

    public override void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Atack", false);

    }

    #region movement
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

    public override void FleeFromPlayer()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 fleePosition = transform.position + directionAwayFromPlayer.normalized * 10f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        resetAnim();
        anim.SetBool("Moving", true);
    }
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(1);
        }
    }

    public override void Escape()
    {
        if (currentlife <= 50)
        {
            StartCoroutine(HealOverTime());

        }
    }

    public override void TakeDamage(float dmg)
    {
        currentlife -= dmg;
        GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
        GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);
        Destroy(debris, 5);
        Destroy(acid, 15);
        if (currentlife <= 0)
        {
            Debug.Log("the skeleton received damage ");
            GameManager.instance.enemys.Remove(this.gameObject);
            if (gameObject.tag == "Chaman")
                SoundManager.instance.PlaySound(chamanDeathClip, transform, 1f, false);
            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }
            GameManager.instance.UnregisterHealer(this);
            Destroy(this.gameObject);
        }
    }

    #region Healing
    public IEnumerator HealOverTime()
    {
        if (isHealing) yield break; // Salir si ya está curando
        isHealing = true;

        float timer = 0f;

        while (timer < healDuration)
        {
            // Curar cada "healInterval"
            HealNearbyEntities();


            yield return new WaitForSeconds(healInterval);

            // Actualizar el temporizador
            timer += healInterval;
        }

    }

    public void HealNearbyEntities()
    {
        var healPosition = transform.position;

        Collider[] colliders = Physics.OverlapSphere(healPosition, healRadius);

        Debug.Log("Healing " + colliders.Length + " entities");

        // Recorrer todos los colisionadores dentro del área
        foreach (var col in colliders)
        {

            if (col.gameObject.layer == 10)
            {
                // Intentar obtener el componente EnemisBehaivor del objeto
                EnemisBehaivor entity = col.GetComponent<EnemisBehaivor>();

                if (entity != null)
                {
                    entity.Healing(healingAmount);
                }
            }
        }
    }

    public override bool HasEnoughNearbyAllies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, healRadius);
        int allyCount = 0;

        foreach (var col in colliders)
        {
            if (col.gameObject.layer == 10) // Asegúrate de usar el layer correcto
            {
                EnemisBehaivor ally = col.GetComponent<EnemisBehaivor>();
                if (ally != null && ally != this) // Ignorar al propio curador
                {
                    allyCount++;
                }
            }
        }

        Debug.Log("Cantidad de aliados cercanos: " + allyCount);
        return allyCount >= 1; // Cambia el número según lo que necesites
    }
    #endregion
}
