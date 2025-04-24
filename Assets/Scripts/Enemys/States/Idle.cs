using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : IState
{

    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _fsm;
    private float _idleTimer;
    private float _idleDuration = 3f;


    public Idle(NavMeshAgent agent, E_Shooter shooter, StateMachine fsm)
    {
        _agent = agent;
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Idle OnEnter");
        //_Shooter.anim.SetBool("Idle", true);
        _Shooter.isIdle = true;
        _agent.speed = 0f; // Detener al enemigo
    }

    public void Tick()
    {
        _idleTimer += Time.deltaTime;

        if (_idleTimer <= _idleDuration)
        {
        Debug.Log("Idle Tick");
            _Shooter.isIdle = false; // Disparador para transición
        }
    }

    public void OnExit()
    {
        _idleTimer = 0f; // Reiniciar el temporizador
        _Shooter.isIdle = false;
        //_Shooter.anim.SetBool("Idle", false);
        Debug.Log("Idle OnExit");
    }
}
