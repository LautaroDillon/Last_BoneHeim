using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{

    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _FSM;

    public Chase(NavMeshAgent agent, E_Shooter shooter, StateMachine fSM)
    {
        _agent = agent;
        _Shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        _agent.speed = _Shooter.walkSpeed;
    }

    public void Tick()
    {
        if (_Shooter.canSeePlayer)
        {
            _Shooter.agent.SetDestination(_Shooter.player.position);
        }

    }

    public void OnExit()
    {

    }
}
