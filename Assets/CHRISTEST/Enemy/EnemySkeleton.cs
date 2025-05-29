using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrolling, Chasing, Attacking }
    public AIState currentState = AIState.Patrolling;

    [Header("References")]
    public Transform player;
    public LayerMask visionMask;
    public AStarManager pathfinder;

    [Header("Stats")]
    public float moveSpeed;

    [Header("Settings")]
    public float detectionRadius = 10f;
    public float attackRange = 2f;
    public float updatePathInterval = 0.5f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int patrolIndex = 0;

    private List<Vector3> currentPath = new List<Vector3>();
    private int pathIndex = 0;
    private float lastPathUpdateTime;

    [Header("Alert Settings")]
    public float alertRadius = 10f;
    private Vector3 lastKnownPlayerPos;

    private void Start()
    {
        if (GameManager.instance != null)
            player = GameManager.instance.player.transform;

        pathfinder = AStarManager.instance;

        GoToNextPatrolPoint();
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

        if (Vector3.Distance(transform.position, patrolPoints[patrolIndex].position) < 1f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            GoToNextPatrolPoint();
        }
    }

    private void GoToNextPatrolPoint()
    {
        currentPath = pathfinder.FindPath(transform.position, patrolPoints[patrolIndex].position);
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

        Attack(); // Placeholder
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
            return;

        Vector3 target = currentPath[pathIndex];
        Vector3 dir = (target - transform.position).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);

        if (Vector3.Distance(transform.position, target) < 0.3f)
        {
            pathIndex++;
        }
    }

    private bool HasLineOfSight()
    {
        Vector3 dir = player.position - transform.position;
        return !Physics.Raycast(transform.position, dir.normalized, dir.magnitude, visionMask);
    }

    private void Attack()
    {
        Debug.Log("Enemy attacks the player!");
        // Add actual attack logic/animation/event
    }

    private void AlertNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (var hit in hits)
        {
            EnemyAI otherAI = hit.GetComponent<EnemyAI>();
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
