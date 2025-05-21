using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : IState
{

    E_Shooter _Shooter;
    StateMachine _fsm;
    private float _idleTimer;
    private float _idleDuration = 3f;


    public Idle( E_Shooter shooter, StateMachine fsm)
    {
        
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Idle OnEnter");
        _Shooter.anim.SetBool("Idle", true);
        _Shooter.isIdle = true;
        _idleTimer = 0f;             // Reiniciar el temporizador
    }

    public void Tick()
    {
        _Shooter.anim.SetFloat("Horizontal", 0, .25f, Time.deltaTime);
        _Shooter.anim.SetFloat("Vertical", 0, .25f, Time.deltaTime);

        _idleTimer += Time.deltaTime;

        if (_idleTimer >= _idleDuration)
        {
            Debug.Log("Idle Tick");
            _Shooter.isIdle = false; // Disparador para transición
        }
    }

    public void OnExit()
    {
        _Shooter.isIdle = false;
        _Shooter.anim.SetBool("Idle", false);
        Debug.Log("Idle OnExit");
        _idleTimer = 0f;            // Reiniciar el temporizador
    }
}
