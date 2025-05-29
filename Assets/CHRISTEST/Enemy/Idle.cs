using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle : IState
{
    private readonly E_Shooter _shooter;
    private readonly StateMachine _fsm;

    private float _idleTimer;
    private float _idleDuration = 3f;

    public Idle(E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Idle OnEnter");
        _shooter.anim.SetBool("Idle", true);
        _shooter.isIdle = true;
        _idleTimer = 0f;
    }

    public void Tick()
    {
        _shooter.anim.SetFloat("Horizontal", 0, 0.25f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", 0, 0.25f, Time.deltaTime);

        _idleTimer += Time.deltaTime;

        if (_idleTimer >= _idleDuration)
        {
            Debug.Log("Idle duration expired");
        }
    }

    public void OnExit()
    {
        Debug.Log("Idle OnExit");
        _shooter.anim.SetBool("Idle", false);
        _shooter.isIdle = false;
        _idleTimer = 0f;
    }
}
