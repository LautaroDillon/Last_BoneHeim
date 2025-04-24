using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Patrol")]
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;
    public bool ispatrolling;


    [Header("Attack")]
    public float Shootcooldown;
    public bool alreadyAttacked;
    public GameObject firePoint;
    public float walkSpeed;
    [SerializeField] private float projectileSpeed;
    public float strafeSpeed = 5f;

    [Header("States")]
    public float chaseDistance, attackRange;
    public bool playerInSightRange, playerInAttackRange, canSeePlayer;
    public bool isIdle;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask obstructionMask;
    [SerializeField] protected float checkRadius;
    [Range(0, 360)]
    [SerializeField] protected float angle;
    public Transform lastpoint;
    #endregion

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
        StartCoroutine(FOVRoutime());

        var idle = new Idle(agent, this, fsm);
        var patrol = new Patrol(agent, this, fsm);
        var chase = new Chase(agent, this, fsm);
        var Search = new Serach_S(agent, this, fsm);
        var strafe = new Strafe(agent, this, fsm);
        var attack = new Atack(agent, this, fsm);
        //var mele = new MeleAtack(agent, this, fsm);
        var death = new Death(agent, this, fsm);

        fsm.SetState(idle);

        // Definir las transiciones
        at(idle, patrol, () => !isIdle);
        at(patrol, idle, () => !ispatrolling);
        at(patrol, chase, () => canSeePlayer && !playerInAttackRange);
        at(chase, attack, () => playerInAttackRange);
        at(attack, strafe, () => !alreadyAttacked); // Ejemplo
        at(strafe, chase, () => alreadyAttacked && !playerInAttackRange);
        at(chase, Search, () => !canSeePlayer);
        at(Search, patrol, () => true); // Despues de buscar vuelve a patrullar
       // at(chase, mele, () => playerInAttackRange);
        //at(mele, chase, () => !playerInAttackRange);
        any(death, () => currentHealth <= 0); // Transición a Death desde cualquier estado
    }


    void at(IState from, IState to, Func<bool> condition) => fsm.AddTransition(from, to, condition);
    void any(IState to, Func<bool> condition) => fsm.AddAnyTransition(to, condition);


    private void Update()
    {
        fsm.Tick();
    }

    public void isinIdle()
    {
        float time = Random.Range(0.5f, 3f);
        time -= Time.deltaTime;

        if (time <= 0)
        {
            //anim.SetBool("Idle", true);
            isIdle = false;
        }

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
                    // Debug.Log("veo player");
                    return true;
                }
                else
                {
                    // Debug.Log(" no veo player");

                    return false;
                }
            }
            else
            {
                //Debug.Log(" no veo player");
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


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            anim.SetBool("isDead", true);
            agent.speed = 0f;
            Death();
        }
    }
    private void Death()
    {
          if (isDead == true)
              Invoke("DestroyEnemy", 0.5f);
    }

    private void DestroyEnemy()
    {
          Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            TakeDamage(50);
            AudioManager.instance.PlaySFX("Bullet Enemy Impact", 1f, false);
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
    }
}
