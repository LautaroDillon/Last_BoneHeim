using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Serach_S : IState
{
    NavMeshAgent _agent;
    E_Shooter _shooter;
    StateMachine _fsm;
    private float _searchDuration = 3f;
    private float _searchTimer;

    private float _waitTimer;
    private float _waitDuration = 2f;

    private bool _movingToNewPoint = false;
    private Vector3 _targetSearchPoint;

    public Serach_S(NavMeshAgent agent, E_Shooter shooter, StateMachine fsm)
    {
        _agent = agent;
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Search OnEnter");
        _searchTimer = 0f;
        _waitTimer = 0f;
        _movingToNewPoint = false;
        _agent.speed = _shooter.walkSpeed * 0.5f; // Reducción de velocidad para hacerlo más deliberado
    }

    public void Tick()
    {
        _searchTimer += Time.deltaTime;

        // Si está atacando, se queda quieto
        if (_shooter.playerInAttackRange)
        {
            _agent.SetDestination(_shooter.transform.position);
            return;
        }

        if (!_movingToNewPoint || _agent.remainingDistance < 0.5f)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _waitDuration)
            {
                _targetSearchPoint = GetRandomSearchPoint();
                _agent.SetDestination(_targetSearchPoint);
                _movingToNewPoint = true;
                _waitTimer = 0f;
            }
        }
    }

    public void OnExit()
    {
        Debug.Log("Search OnExit");
        _agent.ResetPath();
    }

    private Vector3 GetRandomSearchPoint()
    {
        float radius = 5f;
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection.y = 0;
        Vector3 candidate = _shooter.player.position + randomDirection;

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            float distToPlayer = Vector3.Distance(hit.position, _shooter.player.position);
            if (distToPlayer <= _shooter.attackRange)
                return hit.position;
        }
        return _shooter.transform.position;
    }
}
