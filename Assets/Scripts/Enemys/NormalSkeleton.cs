using UnityEngine;
using UnityEngine.AI;

public class NormalSkeleton : EnemisBehaivor
{
    #region variables
    [Header("NavMesh Settings")]
    [SerializeField] private float stoppingDistance;

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
            /*if (agent != null)
            {
                agent.isStopped = false;
                agent.velocity = Vector3.zero;
            }*/

            // Gira hacia el jugador
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
        Debug.Log("Intentando hacer strafe mientras dispara...");

        float shortStrafeDistance = Random.Range(5f, 7f);
        Vector3 strafeDirection = transform.right * (Random.Range(0, 2) == 0 ? 1 : -1);
        Vector3 strafeTarget = transform.position + strafeDirection * shortStrafeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(strafeTarget, out hit, 8f, NavMesh.AllAreas)) // Aumenta radio de b�squeda
        {
            Debug.Log($"Destino de strafe encontrado: {hit.position}");

            agent.isStopped = false; // Asegura que pueda moverse
            agent.ResetPath(); // Borra cualquier ruta anterior
            agent.SetDestination(hit.position);
            agent.speed = 2f; // Asegura que tenga velocidad

            Debug.Log($"Moviendo al enemigo al punto de strafe: {agent.destination}");
        }
        else
        {
            Debug.LogWarning("No se encontr� un punto v�lido en el NavMesh para el strafe.");
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
