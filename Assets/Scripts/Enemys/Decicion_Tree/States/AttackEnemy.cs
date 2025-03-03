using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : IState
{
    FSM _fsm;
    EnemisBehaivor _enemy;

    public AttackEnemy(FSM fsm, EnemisBehaivor enemy)
    {
        _fsm = fsm;
        _enemy = enemy;
    }

    public void OnEnter()
    {
       // Debug.Log("Atacando");
    }
    public void OnUpdate()
    {
        if (_enemy.FieldOfViewCheck())
        {
            _enemy.AttackPlayer();
        }
    }

    public void OnExit()
    {
       // Debug.Log("Saliendo de ataque");
    }

}
