using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleAtack : IState
{

    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _FSM;

    public MeleAtack(NavMeshAgent agent, E_Shooter shooter, StateMachine fSM)
    {
        _agent = agent;
        _Shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {

    }

    public void Tick()
    {

    }

    public void OnExit()
    {

    }
}
