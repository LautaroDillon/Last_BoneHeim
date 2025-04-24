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
        Debug.Log("Chase OnEnter");
        _Shooter.anim.SetBool("Walk", true);
        _agent.speed = _Shooter.walkSpeed;
    }

    public void Tick()
    {
        float distanceToPlayer = Vector3.Distance(_Shooter.transform.position, _Shooter.player.position);
       
        if (_Shooter.canSeePlayer)
        {
            _Shooter.agent.SetDestination(_Shooter.player.position);
        }

        if (distanceToPlayer < _Shooter.attackRange)
        _Shooter.playerInAttackRange = true;

    }

    public void OnExit()
    {
        Debug.Log("Chase OnExit");
        _Shooter.anim.SetBool("Walk", false);
    }
}
