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
        _Shooter.anim.SetTrigger("IsDead");
        _Shooter.agent.isStopped = true; // Detener al enemigo
        _Shooter.Death();
    }

    public void Tick()
    {
        _Shooter.anim.SetFloat("Horizontal", 0, .25f, Time.deltaTime);
        _Shooter.anim.SetFloat("Vertical", 0, .25f, Time.deltaTime);
    }

    public void OnExit()
    {

    }
}
