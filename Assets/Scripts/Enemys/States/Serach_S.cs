using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Serach_S : IState
{
    NavMeshAgent _agent;
    E_Shooter _Shooter;
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
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _searchTimer = 0f;
        _waitTimer = 0f;
        _movingToNewPoint = false;
    }

    public void Tick()
    {
        _searchTimer += Time.deltaTime;

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
        _agent.ResetPath();
    }

    private Vector3 GetRandomSearchPoint()
    {
        float radius = 5f;
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += _Shooter.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return _Shooter.transform.position;
    }
}
