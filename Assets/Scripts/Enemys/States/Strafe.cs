using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Strafe : IState
{
    private NavMeshAgent _agent;
    private E_Shooter _shooter;
    private StateMachine _fsm;

    private float _speedFactor = 1.5f;
    private float _actionDuration = 3f;
    private float _actionTimer;

    private bool _hasChosenMode;
    private bool _moveAwayMode;
    private Vector3 _targetPosition;

    private float _strafeRadius = 15f;
    private float _minSeparation = 4f;

    public Strafe(NavMeshAgent agent, E_Shooter shooter, StateMachine fsm)
    {
        _agent = agent;
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _actionTimer = 0f;
        _hasChosenMode = false;
        _agent.isStopped = false;
        _agent.speed = _shooter.walkSpeed * _speedFactor;
    }

    public void Tick()
    {
        var dir = (_agent.steeringTarget - _shooter.transform.position).normalized;
        var animdir = _shooter.transform.InverseTransformDirection(dir);
        var isfacingmovedirection = Vector3.Dot(dir, _shooter.transform.forward) > 0.5f;

        _shooter.anim.SetFloat("Horizontal", isfacingmovedirection ? animdir.x : 0, .5f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", isfacingmovedirection ? animdir.z : 0, .5f, Time.deltaTime);


        _actionTimer += Time.deltaTime;

        if (!_hasChosenMode)
        {
            _hasChosenMode = true;
            _moveAwayMode = Random.value < 0.5f;

            if (_moveAwayMode)
                AwayTarget();
            else
                LateralTarget();

            _agent.SetDestination(_targetPosition);
        }

        // Marcamos que hemos terminado para que la transición Strafe→Attack funcione
        if (_agent.remainingDistance < 0.5f || _actionTimer >= _actionDuration)
            _shooter.alreadyAttacked = false;
    }

    public void OnExit()
    {
        _agent.speed = _shooter.walkSpeed;
        _agent.isStopped = false;
        _hasChosenMode = false;
    }

    private void AwayTarget()
    {
        Vector3 raw;
        int attempts = 0;
        do
        {
           
            Vector3 dir = (_shooter.transform.position - _shooter.player.position).normalized;
            float dist = Random.Range(_shooter.attackRange * 0.5f, _strafeRadius);
            raw = _shooter.player.position + dir * dist;
            attempts++;
        }
        while (Vector3.Distance(raw, _shooter.transform.position) < _minSeparation && attempts < 10);

        
        if (NavMesh.SamplePosition(raw, out var hit, 1.5f, NavMesh.AllAreas))
            _targetPosition = hit.position;
        else
            _targetPosition = _shooter.transform.position + (raw - _shooter.transform.position).normalized * _minSeparation;
    }

    private void LateralTarget()
    {
        Vector3 ortho = Vector3.Cross(Vector3.up,
            (_shooter.player.position - _shooter.transform.position).normalized
        );
        float dist;
        Vector3 raw;
        int attempts = 0;
        do
        {
            // Distancia aleatoria lateral
            dist = Random.Range(3f, _strafeRadius);
            float sign = Random.value < 0.5f ? 1f : -1f;
            raw = _shooter.player.position + ortho * dist * sign;
            attempts++;
        }
        while (Vector3.Distance(raw, _shooter.transform.position) < _minSeparation && attempts < 10);

        // Clamp al attackRange
        float actualDist = Vector3.Distance(raw, _shooter.player.position);
        if (actualDist > _shooter.attackRange)
            raw = _shooter.player.position +
                  (raw - _shooter.player.position).normalized * (_shooter.attackRange * 0.9f);

        if (NavMesh.SamplePosition(raw, out var hit, 1.5f, NavMesh.AllAreas))
            _targetPosition = hit.position;
        else
            _targetPosition = _shooter.transform.position +
                              (raw - _shooter.transform.position).normalized * _minSeparation;
    }
}
