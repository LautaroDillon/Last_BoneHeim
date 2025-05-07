using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class E_Shooter : Entity
{
    #region variables
    public StateMachine fsm;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public LayerMask whatIsGround, whatIsPlayer;
    public static E_Shooter instance;
    public GameObject bulletDrop;
    public int numberOfBulletsOnDeath = 3;
    public PlayerWeapon playerWeapon;
    public int bulletsToGive = 5;

    [Header("Patrol")]
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public bool isPatrolling;

    [Header("Attack")]
    public float shotCooldown;
    public bool alreadyAttacked;
    public GameObject firePoint;
    public float walkSpeed;
    [SerializeField] private float projectileSpeed;
    public float strafeSpeed = 5f;

    [Header("States")]
    public float chaseDistance, attackRange;
    public bool playerInSightRange, playerInAttackRange, canSeePlayer;
    public bool isIdle = true;
    public bool WasHit;
    public float muchit;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask obstructionMask;
    [SerializeField] protected float checkRadius;
    [Range(0, 360)]
    [SerializeField] protected float angle;
    public Vector3 lastpoint;
    #endregion
    public bool isincombatArena;

    private Vector3 originalPosition;
    public float shakeAmount = 0.05f;  // How much to shake
    public float shakeDuration = 0.1f;

    private void Awake()
    {
        maxHealth = EnemyFlyweight.Shooter.maxLife;
        currentHealth = maxHealth;
        walkSpeed = EnemyFlyweight.Shooter.speed;
        fsm = new StateMachine();
        agent = GetComponent<NavMeshAgent>();
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        originalPosition = transform.position;
        playerWeapon = FindObjectOfType<PlayerWeapon>();

        StartCoroutine(FOVRoutime());

        var idle = new Idle(agent, this, fsm);
        var patrol = new Patrol(agent, this, fsm);
        var chase = new Chase(agent, this, fsm);
        var Search = new Serach_S(agent, this, fsm);
        var strafe = new Strafe(agent, this, fsm);
        var attack = new Atack(agent, this, fsm);
        var death = new Death(agent, this, fsm);
        var Onhit = new OnHit(agent, this, fsm);


        // Definir las transiciones
        at(idle, patrol, () => !isIdle && !isDead);
        at(patrol, idle, () => !isPatrolling && !isDead);
        at(patrol, chase, () => canSeePlayer && !playerInAttackRange && !isDead);
        at(chase, attack, () => playerInAttackRange && !isDead);
        at(attack, strafe, () => alreadyAttacked && playerInAttackRange && !isDead);
        at(attack, chase, () => !playerInAttackRange && !isDead);
        at(attack, Search, () => !playerInAttackRange && !canSeePlayer && !isDead);
        at(strafe, chase, () => !playerInAttackRange && !isDead);
        at(strafe, attack, () => !alreadyAttacked && !isDead);
        at(chase, Search, () => !canSeePlayer && !isDead);
        at(Search, patrol, () => true && !isDead); // Despues de buscar vuelve a patrullar
        any(death, () => currentHealth <= 0 && isDead); // Transición a Death desde cualquier estado
        any(Onhit, () => WasHit && !isDead); // Transición a hit desde cualquier estado
        at(Onhit, idle, () => !WasHit && !isDead); // Transición a idle desde hit
        at(Onhit, patrol, () => !WasHit && !isDead);
        at(Onhit, chase, () => !WasHit && !isDead);
        at(Onhit, Search, () => !WasHit && !isDead);
        at(Onhit, strafe, () => !WasHit && !isDead);
        at(Onhit, attack, () => !WasHit && !isDead); 

        fsm.SetState(idle);
    }


    void at(IState from, IState to, Func<bool> condition) => fsm.AddTransition(from, to, condition);
    void any(IState to, Func<bool> condition) => fsm.AddAnyTransition(to, condition);


    private void Update()
    {
        fsm.Tick();
    }

    #region Fov
    public virtual IEnumerator FOVRoutime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        // Debug.Log("busco player");

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

    #region patrol
    public void Patroling()
    {
        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

          //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    public void SearchWalkPoint()
    {
          //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }
    #endregion

    #region takedamage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(ShakeEffect());
        if (currentHealth > 0 && currentHealth <= (maxHealth/3))
        {
            WasHit = true;
        }
        if (currentHealth <= 0 && !isincombatArena)
        {
            isDead = true;

            agent.speed = 0f;
        }
    }
    public void Death()
    {
        if (isDead == true)
        {
            Invoke("DestroyEnemy", 2.3f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    #endregion

    private IEnumerator ShakeEffect()
    {
        float timeElapsed = 0f;

        while (timeElapsed < shakeDuration)
        {
            // Create a small random shake direction
            Vector3 shakeOffset = new Vector3(
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount),
                Random.Range(-shakeAmount, shakeAmount)
            );

            transform.position = originalPosition + shakeOffset;

            timeElapsed += Time.deltaTime;

            // Wait until the next frame before continuing
            yield return null;
        }

        // Return the enemy to its original position
        transform.position = originalPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance); 
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
    }


    public IEnumerator waitforsecond(float time)
    {
        yield return new WaitForSeconds(time);
        WasHit = false;
    } 
    
    public IEnumerator waittoendAnim(float time)
    {
        yield return new WaitForSeconds(time);
        alreadyAttacked = true;

    }
}
