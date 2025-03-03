using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : IState
{
    FSM _fsm;
    EnemisBehaivor _enemy;

    public Walk(FSM fsm, EnemisBehaivor enemy)
    {
        _fsm = fsm;
        _enemy = enemy;
    }

    public void OnEnter()
    {
        //Debug.Log("enter walk");
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        //Debug.Log("Caminando");
        _enemy.Patrol();
    }
}
