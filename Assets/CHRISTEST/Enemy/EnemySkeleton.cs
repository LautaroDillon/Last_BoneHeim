using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AIState { Patrolling, Chasing, Attacking }
public class EnemySkeleton : Entity
{
    public AIState currentState = AIState.Patrolling;

    [Header("References")]
    public Transform player;
    public LayerMask visionMask;
    public AStarManager pathfinder;
    public NodeGrid nodeGenerator;
    public GameObject bulletDrop;
    public PlayerWeapon playerWeapon;


    [Header("Stats")]
    public float moveSpeed;

    [Header("Settings")]
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float updatePathInterval = 0.5f;
    public float rotationSpeed = 6f;
    public int numberOfBulletsOnDeath;

    [Header("Alert Settings")]
    public float alertRadius = 10f;
    private Vector3 lastKnownPlayerPos;

    private List<Vector3> currentPath = new List<Vector3>();
    private int pathIndex = 0;
    private float lastPathUpdateTime;

    private Node currentPatrolNode;
    public Animator animator;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        numberOfBulletsOnDeath = Random.Range(2, 4);

        maxHealth = EnemyFlyweight.Shooter.maxLife;
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (GameManager.instance != null)
            player = GameManager.instance.player.transform;

        pathfinder = AStarManager.instance;

        if (nodeGenerator == null || nodeGenerator.GeneratedNodes == null || nodeGenerator.GeneratedNodes.Count == 0)
        {
            Debug.LogWarning("No patrol nodes found!");
            return;
        }

        // Snap to nearest node
        Node nearestNode = nodeGenerator.GetClosestGroundNode(transform.position);
        if (nearestNode != null)
        {
            transform.position = nearestNode.Position;
        }

        GoToRandomPatrolNode();
    }

    private void Update()
    {
        if (player == null || pathfinder == null || isDead)
            return;

        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Chasing:
                ChasePlayer();
                break;
            case AIState.Attacking:
                AttackPlayer();
                break;
        }

        if (currentState != AIState.Attacking &&
            Vector3.Distance(transform.position, player.position) <= detectionRadius &&
            HasLineOfSight())
        {
            currentState = AIState.Chasing;
            lastKnownPlayerPos = player.position;
            AlertNearbyEnemies();
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        AudioManager.instance.PlaySFXOneShot("ShooterDamage", 1f);
        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            isDead = true;
            Death();
        }
        else
        {
            animator.SetTrigger("isHit");
        }
    }

    public void Death()
    {
        if (isDead == true)
        {
            AudioManager.instance.PlaySFXOneShot("ShooterDeath", 1f);
            DropBullets();
            // Optional: disable colliders or nav/AI logic
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            Destroy(gameObject);
        }
    }

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

    private void Patrol()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        MoveAlongPath();

        if (Vector3.Distance(transform.position, currentPatrolNode.Position) < 1f)
        {
            GoToRandomPatrolNode();
        }
    }

    private void GoToRandomPatrolNode()
    {
        if (nodeGenerator == null || nodeGenerator.GeneratedNodes.Count == 0)
            return;

        Node newNode;
        do
        {
            newNode = nodeGenerator.GeneratedNodes[Random.Range(0, nodeGenerator.GeneratedNodes.Count)];
        }
        while (newNode == currentPatrolNode && nodeGenerator.GeneratedNodes.Count > 1);

        currentPatrolNode = newNode;
        currentPath = pathfinder.FindPath(transform.position, currentPatrolNode.Position);
        pathIndex = 0;
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            currentState = AIState.Attacking;
            return;
        }

        if (Time.time - lastPathUpdateTime > updatePathInterval)
        {
            currentPath = pathfinder.FindPath(transform.position, player.position);
            pathIndex = 0;
            lastPathUpdateTime = Time.time;
        }

        MoveAlongPath();
        RotateTowardsPlayer();
    }

    private void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            animator.SetBool("IsAttacking", false);
            currentState = AIState.Chasing;
            return;
        }

        RotateTowardsPlayer();
        if (!animator.GetBool("IsAttacking"))
        {
            animator.SetBool("IsAttacking", true);
        }
        Attack();
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 target = currentPath[pathIndex];
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0; // Stay on XZ plane

        float distanceToTarget = toTarget.magnitude;

        // Advance to next path point if we're close enough
        if (distanceToTarget < 0.3f)
        {
            pathIndex++;
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 moveDir = toTarget.normalized;

        // Smooth rotation
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Smooth movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 velocity = moveDir * moveSpeed;
            Vector3 newPosition = rb.position + velocity * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        animator.SetFloat("Speed", moveSpeed);
    }

    private bool HasLineOfSight()
    {
        Vector3 dir = player.position - transform.position;
        return !Physics.Raycast(transform.position, dir.normalized, dir.magnitude, visionMask);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Keep rotation horizontal only

        if (directionToPlayer.sqrMagnitude > 0.001f) // avoid zero direction
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        Debug.Log("Enemy attacks the player!");
        // Add attack logic, animation, or event here
    }

    private void AlertNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (var hit in hits)
        {
            EnemySkeleton otherAI = hit.GetComponent<EnemySkeleton>();
            if (otherAI != null && otherAI != this)
            {
                otherAI.OnAlerted(player.position);
            }
        }
    }

    public void OnAlerted(Vector3 playerPosition)
    {
        if (currentState == AIState.Patrolling)
        {
            lastKnownPlayerPos = playerPosition;
            currentState = AIState.Chasing;
            currentPath = pathfinder.FindPath(transform.position, lastKnownPlayerPos);
            pathIndex = 0;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }
}
