using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AIState { Patrolling, Chasing, Attacking }
public class EnemySkeleton : MonoBehaviour
{
    public AIState currentState = AIState.Patrolling;

    [Header("References")]
    public Transform player;
    public LayerMask visionMask;
    public AStarManager pathfinder;
    public NodeGrid nodeGenerator;

    [Header("Stats")]
    public float moveSpeed;

    [Header("Settings")]
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float updatePathInterval = 0.5f;

    [Header("Alert Settings")]
    public float alertRadius = 10f;
    private Vector3 lastKnownPlayerPos;

    private List<Vector3> currentPath = new List<Vector3>();
    private int pathIndex = 0;
    private float lastPathUpdateTime;

    private Node currentPatrolNode;

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

        GoToRandomPatrolNode();
    }

    private void Update()
    {
        if (player == null || pathfinder == null)
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

        // Transition to chasing if player is detected
        if (currentState != AIState.Attacking &&
            Vector3.Distance(transform.position, player.position) <= detectionRadius &&
            HasLineOfSight())
        {
            currentState = AIState.Chasing;
            lastKnownPlayerPos = player.position;
            AlertNearbyEnemies();
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
    }

    private void AttackPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            currentState = AIState.Chasing;
            return;
        }

        Attack();
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
            return;

        Vector3 target = currentPath[pathIndex];
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0; // Stay on XZ plane

        float distanceToTarget = toTarget.magnitude;

        // Advance to next path point if we're close enough
        if (distanceToTarget < 0.3f)
        {
            pathIndex++;
            return;
        }

        Vector3 moveDir = toTarget.normalized;

        // Smooth rotation
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 6f * Time.deltaTime);
        }

        // Smooth movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 velocity = moveDir * moveSpeed;
            Vector3 newPosition = rb.position + velocity * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        /* if (currentPath == null || pathIndex >= currentPath.Count)
             return;

         Vector3 target = currentPath[pathIndex];
         Vector3 horizontalDir = (target - transform.position);
         horizontalDir.y = 0; // only move in XZ plane

         if (horizontalDir.magnitude < 0.1f)
         {
             pathIndex++;
             return;
         }

         Vector3 moveDir = horizontalDir.normalized;
         Vector3 newPosition = transform.position + moveDir * moveSpeed * Time.deltaTime;

         // Ground alignment
         RaycastHit hit;
         Vector3 origin = transform.position + Vector3.up * 1f; // start raycast above the enemy
         if (Physics.Raycast(origin, Vector3.down, out hit, 3f))
         {
             newPosition.y = hit.point.y; // snap to ground

             // Apply movement using Rigidbody
             Rigidbody rb = GetComponent<Rigidbody>();
             if (rb != null)
             {
                 rb.MovePosition(newPosition);

                 // Rotate to face movement direction
                 if (moveDir != Vector3.zero)
                 {
                     Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                     rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.deltaTime));
                 }
             }
             else
             {
                 Debug.LogWarning("Enemy has no Rigidbody component!");
             }
         }
         else
         {
             Debug.LogWarning("AI lost ground! Possibly stepping off the map.");
         }

         // Advance to next path point if close
         if (Vector3.Distance(transform.position, target) < 0.5f)
         {
             pathIndex++;
         }
        */
    }

    private bool HasLineOfSight()
    {
        Vector3 dir = player.position - transform.position;
        return !Physics.Raycast(transform.position, dir.normalized, dir.magnitude, visionMask);
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
