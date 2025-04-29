using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OnHit : IState
{
    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _fsm;
    private float _idleTimer;
    private float _idleDuration = 3f;


    public OnHit(NavMeshAgent agent, E_Shooter shooter, StateMachine fsm)
    {
        _agent = agent;
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
       _Shooter.StartCoroutine(_Shooter.waitforsecond(0.1f));
        _Shooter.anim.SetTrigger("Hit");
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
