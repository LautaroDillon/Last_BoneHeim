using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Death : IState
{

    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _fsm;

    public Death(NavMeshAgent agent, E_Shooter shooter, StateMachine fsm)
    {
        _agent = agent;
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _Shooter.anim.SetBool("Death", true);
        _Shooter.agent.isStopped = true; // Detener al enemigo
        _Shooter.Death();
    }

    public void Tick()
    {

    }

    public void OnExit()
    {

    }
}
