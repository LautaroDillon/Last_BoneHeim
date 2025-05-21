using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class E_Shooter : Entity
{
    #region variables
    public StateMachine fsm;

    [Header("References")]
    public Transform player;
    public Animator anim;
    public LayerMask whatIsGround, whatIsPlayer;
    public static E_Shooter instance;
    public GameObject bulletDrop;
    public int numberOfBulletsOnDeath;
    public PlayerWeapon playerWeapon;

    [Header("Attack")]
    public float shotCooldown;
    public bool alreadyAttacked;
    public GameObject firePoint;
    [SerializeField] private float projectileSpeed;
    public float strafeSpeed = 5f;
    public bool wasKilledByThrowable = false;

    [Header("States")]
    public float chaseDistance, attackRange;
    public bool playerInSightRange, playerInAttackRange, canSeePlayer;
    public bool isIdle = true;
    public bool WasHit;
    public bool isPatrolling;
    public bool otherSeenPlayer;
    public float muchit;

    [Header("Player Detection")]
    [SerializeField] public LayerMask obstructionMask;
    [SerializeField] protected float checkRadius;
    [Range(0, 360)]
    [SerializeField] protected float angle;
    public Vector3 lastpoint;

    [Header("Nodes")]
    public NodePathfinding initialNode;
    public NodePathfinding goalNode;
    public List<NodePathfinding> path;
    public int pathIndex;
    public float nodeReachDistance = 0.3f;

    public float moveSpeed;
    public float maxSpeed;
    public float arriveRadius;
    public float maxForce;
    [HideInInspector] public Vector3 velocity;

    [Header("Zona")]
    public int zoneId;

    [Header("search")]
    public Vector3 lastposition;

    #endregion
    public bool isincombatArena;
    public float lastAttackTime;

    public int groupIndex;

    public Rigidbody rb;

    private void Awake()
    {
        maxHealth = EnemyFlyweight.Shooter.maxLife;
        currentHealth = maxHealth;
        numberOfBulletsOnDeath = Random.Range(1, 4);
        rb = GetComponent<Rigidbody>();
        fsm = new StateMachine();
        if (instance == null)
            instance = this;
    }

    #region start
    private void Start()
    {
        GameManager.instance.RegisterShooter(this);

        playerWeapon = FindObjectOfType<PlayerWeapon>();

        StartCoroutine(FOVRoutime());

        var idle = new Idle(this, fsm);
        var patrol = new Patrol(this, fsm);
        var chase = new Chase(this, fsm);
        var Search = new Serach_S(this, fsm);
        var strafe = new Strafe(this, fsm);
        var attack = new Atack(this, fsm);
        var death = new Death(this, fsm);
        var Onhit = new OnHit(this, fsm);


        // Definir las transiciones
        at(idle, patrol, () => !isIdle && !isDead);
        at(patrol, idle, () => !isPatrolling && !isDead);
        at(patrol, chase, () => canSeePlayer && !playerInAttackRange && !isDead || otherSeenPlayer && !playerInAttackRange && !isDead);
        at(patrol, attack, () => canSeePlayer && playerInAttackRange && !isDead);
        at(chase, attack, () => playerInAttackRange && !isDead);
        at(attack, chase, () => !playerInAttackRange && !isDead && canSeePlayer && otherSeenPlayer);
        at(attack, Search, () => !canSeePlayer && !isDead && lastposition != Vector3.zero && otherSeenPlayer);
        at(chase, Search, () => !canSeePlayer && !isDead && lastposition != Vector3.zero && otherSeenPlayer);
        at(Onhit, strafe, () => !WasHit && !isDead);
        at(strafe, chase, () => !playerInAttackRange && !isDead && canSeePlayer && otherSeenPlayer);
        at(attack, strafe, () => alreadyAttacked && playerInAttackRange && !isDead);
        at(strafe, attack, () => !alreadyAttacked && !isDead);
        at(Search, patrol, () => isPatrolling && !isDead);       // Despues de buscar vuelve a patrullar
        at(Onhit, idle, () => !WasHit && !isDead);       // Transición a idle desde hit
        at(Onhit, patrol, () => !WasHit && !isDead);
        at(Onhit, chase, () => !WasHit && !isDead);
        at(Onhit, Search, () => !WasHit && !isDead);
        at(Onhit, attack, () => !WasHit && !isDead && lastposition != Vector3.zero);

        any(death, () => currentHealth <= 0 && isDead);  // Transición a Death desde cualquier estado
        any(Onhit, () => WasHit && !isDead);             // Transición a hit desde cualquier estado

        fsm.SetState(idle);
    }


    void at(IState from, IState to, Func<bool> condition) => fsm.AddTransition(from, to, condition);
    void any(IState to, Func<bool> condition) => fsm.AddAnyTransition(to, condition);
    #endregion

    private void Update()
    {
        fsm.Tick();
    }

    #region Fov
    public virtual IEnumerator FOVRoutime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            canSeePlayer = FieldOfViewCheck();
        }
    }
    public virtual bool FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, checkRadius, whatIsPlayer);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    lastposition = player.position;
                    if (otherSeenPlayer == false)
                    {
                        GameManager.instance.oneseeplayer(zoneId);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            //Debug.Log(" no veo player");
            return false;
        }
    }
    #endregion

    #region Movement
    public Vector3 Seek(Vector3 targetSeek)
    {
        var desired = targetSeek - transform.position;
        desired.Normalize();
        desired *= maxSpeed;
        return CalculateSteering(desired);
    }
    public Vector3 CalculateSteering(Vector3 desired)
    {
        var steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering;
    }

    public void AddForce(Vector3 dir)
    {
        velocity += dir;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }

    public Vector3 ObstacleAvoidance()
    {
        Vector3 pos = transform.position;
        Vector3 dir = transform.forward;
        float dist = velocity.magnitude; //Que tan rapido estoy yendo

        Debug.DrawLine(pos, pos + (dir * dist));

        if (Physics.SphereCast(pos, 1, dir, out RaycastHit hit, dist, obstructionMask))
        {
            var obstacle = hit.transform; //Obtengo el transform del obstaculo q acaba de tocar
            Vector3 dirToObject = obstacle.position - transform.position; //La direccion del obstaculo

            float anguloEntre = Vector3.SignedAngle(transform.forward, dirToObject, Vector3.up); //(Dir. hacia donde voy, Dir. objeto, Dir. mis costados)

            Vector3 desired = anguloEntre >= 0 ? -transform.right : transform.right; //Me meuvo para derecha o izquierda dependiendo donde esta el obstaculo
            desired.Normalize();
            desired *= maxSpeed;

            return CalculateSteering(desired);
        }

        return Vector3.zero;
    }

    #endregion

    #region takedamage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0 && currentHealth <= (maxHealth / 3))
        {
            WasHit = true;
        }
        if (currentHealth <= 0 && !isincombatArena)
        {
            isDead = true;

        }
    }

    public void Death()
    {
        if (isDead == true)
        {
            AudioManager.instance.PlaySFXOneShot("ShooterDeath", 1f);
            DropBullets();
            Invoke("DestroyEnemy", 2.3f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    #endregion

    void DropBullets()
    {
        for (int i = 0; i < numberOfBulletsOnDeath; i++)
        {
            Vector3 dropOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                0.5f,
                Random.Range(-0.3f, 0.3f)
            );

            Vector3 dropPosition = transform.position + dropOffset;

            GameObject bullet = Instantiate(bulletDrop, dropPosition, Quaternion.identity);

            bullet.transform.position += Vector3.up * 0.3f;

            // Add physics force in random arc
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    1f,
                    Random.Range(-1f, 1f)
                ).normalized;

                float dropForce = Random.Range(3f, 6f);
                rb.AddForce(randomDirection * dropForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }

    public IEnumerator waitforsecond(float time)
    {
        AudioManager.instance.PlaySFXOneShot("ShooterDamage", 1f);
        yield return new WaitForSeconds(time);
        WasHit = false;
    }

    public IEnumerator longtime(float time)
    {
        yield return new WaitForSeconds(time);
        otherSeenPlayer = false;

    }
}
