using UnityEngine;
using UnityEngine.AI;

public class EMelee : EnemisBehaivor
{
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

    [Header("NavMesh")]
    private NavMeshAgent navMeshAgent;
    //Transform Player;

    [Header("Sounds")]
    [SerializeField] private AudioClip swordSlashClip;
    [SerializeField] private AudioClip knightDeathClip;
    [SerializeField] private AudioClip bulletImpactClip;

    GameObject acid;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
       // Player = GameManager.instance.thisIsPlayer;
        currentlifeShield = lifeShield;
        hasshield = true;
    }

    private void Start()
    {
        //GeneratePatrolPoint();
        StartCoroutine(FOVRoutime());

    }

    public void resetAnim()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Atack", false);
        anim.SetBool("Idle", false);
    }

    private void Update()
    {
        if (currentlife <= 0)
        {
            navMeshAgent.isStopped = true;
            return;
        }

        if (canSeePlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            //Patrol(); // Si decides habilitar la patrulla, también usará rotación.
        }

    }

    private void RotateTowardsMovement()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f) // Si se está moviendo
        {
            Vector3 direction = navMeshAgent.velocity.normalized;
            direction.y = 0; // Asegurarse de no cambiar la inclinación vertical
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ChasePlayer()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);
        RotateTowardsMovement();
        resetAnim();
        anim.SetBool("Walk", true);
    }

    private void AttackPlayer()
    {
        navMeshAgent.isStopped = true;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            resetAnim();
            //anim.SetBool("Atack", true);
            anim.SetTrigger("Attack");
            player.GetComponent<Idamagable>().TakeDamage(attackDamage);
            SoundManager.instance.PlaySound(swordSlashClip, transform, 1f, false);
            lastAttackTime = Time.time;
        }

        Invoke(nameof(ResumeMovement), 1.5f);
    }

    private void ResumeMovement()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false;
        }
    }

    public override void TakeDamage(float dmg)
    {
        healParticle.SetActive(false);
        SoundManager.instance.PlaySound(bulletImpactClip, transform, 1f, false);

        if (hasshield)
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

            navMeshAgent.enabled = false; // Desactivar movimiento al morir
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
