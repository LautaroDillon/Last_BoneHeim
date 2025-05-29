using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Death : IState
{

    
    E_Shooter _Shooter;
    StateMachine _fsm;

    public Death( E_Shooter shooter, StateMachine fsm)
    {
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _Shooter.anim.SetTrigger("IsDead");
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
