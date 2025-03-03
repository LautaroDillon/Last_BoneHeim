using UnityEngine;
using UnityEngine.AI;

public class EMelee : EnemisBehaivor
{
    #region Variables
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
    public GameObject ShieldOBJ;
    public float lifeShield;
    float currentlifeShield;
    public bool hasshield = true;

    [Header("Detection")]
    public float detectionRange;

    [Header("Sounds")]
    [SerializeField] private AudioClip swordSlashClip;
    [SerializeField] private AudioClip knightDeathClip;
    [SerializeField] private AudioClip bulletImpactClip;

    GameObject acid;
    #endregion

    #region basics
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
       // Player = GameManager.instance.thisIsPlayer;
        currentlifeShield = lifeShield;
        hasshield = true;
        firstNode = GameManager.instance.firstquestion;

        fsm = new FSM();
        fsm.CreateState("Attack", new AttackEnemy(fsm, this));
        fsm.CreateState("Escape", new Escape(fsm, this));
        fsm.CreateState("Walk", new Walk(fsm, this));
        fsm.ChangeState("Walk");
    }

    private void Start()
    {
        //GeneratePatrolPoint();
        StartCoroutine(FOVRoutime());

    }
    private void Update()
    {
        /*if (currentlife <= 0)
        {
            navMeshAgent.isStopped = true;
            return;
        }*/

        firstNode.Execute(this);
        fsm.Execute();

    }
    #endregion

    public override void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Atack", false);
        anim.SetBool("Idle", false);
    }

    #region movement
    private void RotateTowardsMovement()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) // Si se está moviendo
        {
            Vector3 direction = agent.velocity.normalized;
            direction.y = 0; // Asegurarse de no cambiar la inclinación vertical
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
        RotateTowardsMovement();
        resetAnim();
        anim.SetBool("Walk", true);
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
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            resetAnim();
            anim.SetBool("Walk", false);

            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime) // Si el temporizador supera el tiempo de espera
            {
                isPatrolling = false;  // Permite generar un nuevo destino
                waitTimer = 0f;        // Reinicia el temporizador
                agent.isStopped = false;
            }
        }
    }

    private void ResumeMovement()
    {
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
    }

    public override void Escape()
    {
        MoveToHealer();
    }
    #endregion

    public override void AttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        agent.isStopped = true;

        if (attackRange < distanceToPlayer)
        {
            ChasePlayer();
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Atacando");
            resetAnim();
            anim.SetBool("Atack", true);
            player.GetComponent<Idamagable>().TakeDamage(attackDamage);
            SoundManager.instance.PlaySound(swordSlashClip, transform, 1f, false);
            lastAttackTime = Time.time;
        }

        Invoke(nameof(ResumeMovement), 1.5f);
    }

    public override void TakeDamage(float dmg)
    {
        healParticle.SetActive(false);
        SoundManager.instance.PlaySound(bulletImpactClip, transform, 1f, false);

        if (!hasshield)
        {
            Debug.Log("pego enemigo");
            currentlife -= dmg;
            acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
            GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);

            Destroy(acid, 15);
            Destroy(debris, 5);
        }
        else
        {
            Debug.Log("pego escudo");
            currentlifeShield -= dmg;
        }

        if (currentlifeShield <= 0)
        {
            ShieldOBJ.SetActive(false);
            hasshield = false;
        }

        if (currentlife <= 0)
        {
            resetAnim();

            agent.enabled = false; // Desactivar movimiento al morir
            GameManager.instance.enemys.Remove(this.gameObject);

            if (gameObject.tag == "Skeleton")
                SoundManager.instance.PlaySound(skeletonDeathClip, transform, 1f, false);

            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }

            Destroy(acid);
            anim.SetBool("Death", true);
            Destroy(this.gameObject, 3f);
            PlayerHealth.instance.life += 10;
            Guns.instance.bulletsLeft += Random.Range(1, 3);
        }
    }
}
