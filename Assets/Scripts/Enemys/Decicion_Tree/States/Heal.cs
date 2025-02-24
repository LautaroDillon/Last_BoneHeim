using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : IState
{
    FSM _fsm;
    EHealer _enemy;

    public Heal(FSM fsm, EHealer enemy)
    {
        _fsm = fsm;
        _enemy = enemy;
    }

    public void OnEnter()
    {
        Debug.Log("Heal");
        _enemy.StartCoroutine(_enemy.HealOverTime());
        //_enemy.anim.SetBool("Healing", true);
    }

    public void OnUpdate()
    {
        if (!_enemy.HasEnoughNearbyAllies())
        {
            Debug.Log("No tiene suficientes aliados cerca");
            _fsm.ChangeState("Walk");
        }
    }

    public void OnExit()
    {
        //_enemy.anim.SetBool("Healing", false);
    }
}
