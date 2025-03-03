using UnityEngine;
using UnityEngine.AI;

public class NormalSkeleton : EnemisBehaivor
{
    #region variables
    [Header("NavMesh Settings")]
    [SerializeField] private float stoppingDistance;

    [Header("Patrol Settings")]
    public float patrolRadius;
    public float waitTime;
    private float waitTimer;
    private Vector3 patrolPoint;
    private bool isPatrolling;

    [Header("Attack Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootRange;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private float fireRate;
    [SerializeField] private float projectileSpeed;

    [Header("Movement Settings")]
    public bool isTurret;
    private bool isAttacking;
    private float nextFireTime;

    [Header("Strafe Settings")]
    [SerializeField] private float strafeDistance = 2.5f;
    [SerializeField] private float strafeCooldown = 1.5f;
    private bool canStrafe = true;

    private Vector3 currentStrafeTarget;
    private bool isStrafing;

    #endregion

    #region basics
    private void Awake()
    {
        currentlife = FlyweightPointer.Eshoot.maxLife;
        agent = GetComponent<NavMeshAgent>();
        instance = this;
        firstNode = GameManager.instance.firstquestion;


        fsm = new FSM();
        fsm.CreateState("Attack", new AttackEnemy(fsm, this));
        fsm.CreateState("Escape", new Escape(fsm, this));
        fsm.CreateState("Walk", new Walk(fsm, this));
        fsm.ChangeState("Walk");

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }

    private void Update()
    {
        firstNode.Execute(this);
        fsm.Execute();
    }
    #endregion

    public override void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Atack", false);
    }

    #region patrol
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
    #endregion

    #region Attack
    public override void AttackPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > shootRange && !isAttacking && !isTurret)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer <= shootRange)
        {

            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookPos), 360 * Time.deltaTime);

            resetAnim();
            anim.SetBool("Atack", true);

            if (CanFire())
            {
                Shoot();
            }

            // Inicia el strafe continuo mientras dispara
            if (!isStrafing)
            {
                isStrafing = true;
                InvokeRepeating(nameof(StrafeWhileShooting), 0f, strafeCooldown);
            }
        }
    }

    private bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    private void Shoot()
    {
        Vector3 directionToPlayer = (player.transform.position - firePoint.position).normalized;

        var bullet = BuletManager.instance.GetBullet();
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.forward = directionToPlayer;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * projectileSpeed;
        }

        nextFireTime = Time.time + 1f / fireRate;
    }
    #endregion

    #region Strafe Movement
    private void StrafeWhileShooting()
    {
        //Intentando hacer strafe mientras dispara

        float shortStrafeDistance = Random.Range(5f, 7f);
        Vector3 strafeDirection = transform.right * (Random.Range(0, 2) == 0 ? 1 : -1);
        Vector3 strafeTarget = transform.position + strafeDirection * shortStrafeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(strafeTarget, out hit, 8f, NavMesh.AllAreas)) // Aumenta radio de búsqueda
        {
            //Debug.Log($"Destino de strafe encontrado: {hit.position}");

            agent.isStopped = false; // Asegura que pueda moverse
            agent.ResetPath(); // Borra cualquier ruta anterior
            agent.SetDestination(hit.position);
            agent.speed = 2f; // Asegura que tenga velocidad

           // Debug.Log($"Moviendo al enemigo al punto de strafe: {agent.destination}");
        }
        else
        {
            Debug.LogWarning("No se encontró un punto válido en el NavMesh para el strafe.");
        }
    }
    #endregion

    public void ChasePlayer()
    {
        if (agent != null)
        {
            resetAnim();
            anim.SetBool("Walk", true);

            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }

        // Detenemos el strafe si empieza a moverse hacia el jugador
        isStrafing = false;
        CancelInvoke(nameof(StrafeWhileShooting));
    }
}
