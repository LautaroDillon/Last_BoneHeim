using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AIState { Patrolling, Chasing, Attacking, Dead }

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
    public GameObject attackColliderPrefab;
    public Transform attackSpawnPoint;

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
    private float patrolWaitTime = 2f;
    private float patrolWaitTimer = 0f;
    private bool waitingAtNode = false;

    private List<Vector3> currentPath = new List<Vector3>();
    private int pathIndex = 0;
    private float lastPathUpdateTime = 0f; // Initialize to zero for first update
    private bool isHit = false;

    private Node currentPatrolNode;
    public Animator animator;
    private bool isAttacking = false;
    private bool isAttackInProgress = false;

    private Rigidbody rb;

    private void Awake()
    {
        numberOfBulletsOnDeath = Random.Range(2, 4);

        maxHealth = EnemyFlyweight.Shooter.maxLife;
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Ensure movement works

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (GameManager.instance != null)
            player = GameManager.instance.player.transform;

        pathfinder = AStarManager.instance;

        playerWeapon = FindObjectOfType<PlayerWeapon>();

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
            currentPatrolNode = nearestNode;
        }

        GoToRandomPatrolNode();
    }

    private void Update()
    {
        if (player == null || pathfinder == null || currentState == AIState.Dead)
            return;

        if (isAttackInProgress)
            return;

        // Usual state machine
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

        // Only check transitions if NOT attacking or attack finished
        if (!isAttackInProgress)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            bool canSeePlayer = HasLineOfSight();

            if (currentState == AIState.Patrolling && distanceToPlayer <= detectionRadius && canSeePlayer)
            {
                currentState = AIState.Chasing;
                lastKnownPlayerPos = player.position;
                AlertNearbyEnemies();
            }
            else if (currentState == AIState.Attacking && (distanceToPlayer > detectionRadius || !canSeePlayer))
            {
                animator.SetBool("isAttacking", false);
                isAttacking = false;
                currentState = AIState.Patrolling;
                GoToRandomPatrolNode();
            }
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (currentState == AIState.Dead)
            return;

        base.TakeDamage(dmg); // reduces health

        AudioManager.instance.PlaySFXOneShot("ShooterDamage", 1f);

        if (isDead)
        {
            currentState = AIState.Dead;
            animator.SetTrigger("isDead");
            Death();
        }
        else
        {
            isHit = true;
            animator.SetTrigger("Hit");
        }
    }

    private void Death()
    {
        AudioManager.instance.PlaySFXOneShot("ShooterDeath", 1f);
        DropBullets();

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        if (rb != null)
            rb.isKinematic = true;

        Invoke(nameof(DestroyEnemy), 2f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void DropBullets()
    {
        for (int i = 0; i < numberOfBulletsOnDeath; i++)
        {
            Vector3 dropOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                0.5f,
                Random.Range(-0.3f, 0.3f)
            );

            Vector3 dropPosition = transform.position + dropOffset + Vector3.up * 0.3f;

            GameObject bullet = Instantiate(bulletDrop, dropPosition, Quaternion.identity);

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    1f,
                    Random.Range(-1f, 1f)
                ).normalized;

                float dropForce = Random.Range(3f, 6f);
                bulletRb.AddForce(randomDirection * dropForce, ForceMode.Impulse);
                bulletRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }

    private void Patrol()
    {
        if (waitingAtNode)
        {
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= patrolWaitTime)
            {
                waitingAtNode = false;
                patrolWaitTimer = 0f;
                GoToRandomPatrolNode();
            }
            return;
        }

        if (currentPath == null || currentPath.Count == 0)
            return;

        MoveAlongPath();

        // ADD NULL CHECK HERE
        if (currentPatrolNode != null && Vector3.Distance(transform.position, currentPatrolNode.Position) < 1f)
        {
            waitingAtNode = true;
            animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
        }
    }

    private void GoToRandomPatrolNode()
    {
        if (nodeGenerator == null || nodeGenerator.GeneratedNodes.Count == 0)
            return;

        float minDistance = 5f; // Minimum distance between nodes to patrol

        Node newNode = null;
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            Node candidate = nodeGenerator.GeneratedNodes[Random.Range(0, nodeGenerator.GeneratedNodes.Count)];
            float distance = Vector3.Distance(candidate.Position, transform.position);

            if (distance >= minDistance)
            {
                newNode = candidate;
                break;
            }
        }

        // Fallback in case no distant enough node is found
        if (newNode == null)
            newNode = nodeGenerator.GeneratedNodes[Random.Range(0, nodeGenerator.GeneratedNodes.Count)];

        currentPatrolNode = newNode;

        currentPath = pathfinder.FindPath(transform.position, currentPatrolNode.Position);
        pathIndex = 0;

        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("No path found to patrol node!");
        }
    }

    private void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            currentState = AIState.Attacking;
            animator.SetBool("isAttacking", false); // Reset attack animation trigger for fresh start

            // Freeze Rigidbody movement on attack start
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
            return;
        }

        // Prevent movement while attacking (if chase somehow still called)
        if (isAttacking)
        {
            animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
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
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRadius || !HasLineOfSight())
        {
            animator.SetBool("isAttacking", false);
            isAttackInProgress = false;  // reset in case
            currentState = AIState.Patrolling;
            GoToRandomPatrolNode();
            return;
        }

        if (distance > attackRange)
        {
            animator.SetBool("isAttacking", false);
            isAttackInProgress = false;  // reset in case
            currentState = AIState.Chasing;
            return;
        }

        RotateTowardsPlayer();

        if (!isAttackInProgress)
        {
            isAttackInProgress = true;
            currentState = AIState.Attacking;
            animator.SetBool("isAttacking", true);
        }

        Attack();
    }


    private void MoveAlongPath()
    {
        if (isAttacking)
        {
            // Freeze movement during attack
            animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
            return;
        }

        if (currentPath == null || pathIndex >= currentPath.Count)
        {
            animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
            return;
        }

        Vector3 target = currentPath[pathIndex];
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0;

        float distanceToTarget = toTarget.magnitude;

        if (distanceToTarget < 0.3f)
        {
            pathIndex++;
            animator.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
            return;
        }

        Vector3 moveDir = toTarget.normalized;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (rb != null)
        {
            Vector3 velocity = moveDir * moveSpeed;
            Vector3 newPosition = rb.position + velocity * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            // Fallback to transform movement if no Rigidbody
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        float horizontal = Vector3.Dot(transform.right, moveDir);
        float vertical = Vector3.Dot(transform.forward, moveDir);

        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
    }

    private bool HasLineOfSight()
    {
        Vector3 dir = player.position - transform.position;
        return !Physics.Raycast(transform.position, dir.normalized, dir.magnitude, visionMask);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void Attack()
    {
        Debug.Log("Enemy attacks the player!");
    }

    public void EndAttack()
    {
        isAttacking = false;
        isAttackInProgress = false;
        animator.SetBool("isAttacking", false);

        if (rb != null)
            rb.isKinematic = false;

        if (player == null)
        {
            currentState = AIState.Patrolling;
            GoToRandomPatrolNode();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && HasLineOfSight())
        {
            currentState = AIState.Attacking;
        }
        else if (distance <= detectionRadius)
        {
            currentState = AIState.Chasing;
        }
        else
        {
            currentState = AIState.Patrolling;
            GoToRandomPatrolNode();
        }
    }

    public void SpawnAttackCollider()
    {
        if (attackColliderPrefab != null)
        {
            Vector3 spawnPos = attackSpawnPoint != null ? attackSpawnPoint.position : transform.position + transform.forward;
            Quaternion spawnRot = transform.rotation;

            Instantiate(attackColliderPrefab, spawnPos, spawnRot);
            Debug.Log("Spawned attack collider from animation event");
        }
    }

    private void AlertNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (var hit in hits)
        {
            EnemySkeleton otherAI = hit.GetComponent<EnemySkeleton>();
            if (otherAI != null && otherAI != this && otherAI.currentState == AIState.Patrolling)
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (currentPath != null && currentPath.Count > 1)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            }
        }
    }
}
