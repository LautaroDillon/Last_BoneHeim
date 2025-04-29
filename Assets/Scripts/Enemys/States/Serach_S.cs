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


        var dir = (_agent.steeringTarget - _Shooter.transform.position).normalized;
        var animdir = _Shooter.transform.InverseTransformDirection(dir);
        var isfacingmovedirection = Vector3.Dot(dir, _Shooter.transform.forward) > 0.5f;

        _Shooter.anim.SetFloat("Horizontal", isfacingmovedirection ? animdir.x : 0, .5f, Time.deltaTime);
        _Shooter.anim.SetFloat("Vertical", isfacingmovedirection ? animdir.z : 0, .5f, Time.deltaTime);

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
