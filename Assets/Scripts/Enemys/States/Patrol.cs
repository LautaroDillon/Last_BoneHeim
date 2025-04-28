using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : IState
{

    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _FSM;
    private float stuckTimer = 0f;
    private float stuckThreshold = 4f;

    public Patrol(NavMeshAgent agent, E_Shooter shooter, StateMachine fSM)
    {
        _agent = agent;
        _Shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        //Debug.Log("Patrol OnEnter");
        _Shooter.anim.SetBool("Walk", true);
        _agent.speed = _Shooter.walkSpeed;
        _Shooter.isPatrolling = true;
        _Shooter.isIdle = false;
        _Shooter.SearchWalkPoint();
        _Shooter.Patroling();


    }

    public void Tick()
    {
        _Shooter.Patroling();

        if (_agent.velocity.magnitude < 0.1f)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer > stuckThreshold)
        {
            //Debug.Log("Parece atascado, buscando nuevo punto");
            _Shooter.SearchWalkPoint();
            stuckTimer = 0f;
        }
    }

    public void OnExit()
    {
       // Debug.Log("Patrol OnExit");
        _Shooter.anim.SetBool("Walk", false);
        _agent.speed = 0f;
        _Shooter.isPatrolling = false;
    }
}
