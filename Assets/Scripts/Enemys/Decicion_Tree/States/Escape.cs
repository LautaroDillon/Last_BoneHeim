using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : IState
{
    FSM _fsm;
    EnemisBehaivor _enemy;

    public Escape(FSM fsm, EnemisBehaivor enemy)
    {
        _fsm = fsm;
        _enemy = enemy;
    }

    public void OnEnter()
    {

    }

    public void OnUpdate()
    {
        _enemy.Escape();
    }

    public void OnExit()
    {

    }
}
