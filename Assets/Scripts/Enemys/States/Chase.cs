using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{


    E_Shooter _Shooter;
    StateMachine _FSM;

    public Chase( E_Shooter shooter, StateMachine fSM)
    {

        _Shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        Debug.Log("Chase OnEnter");
        _Shooter.anim.SetBool("Walk", true);
    }

    public void Tick()
    {


       /* var dir = (_agent.steeringTarget - _Shooter.transform.position).normalized;
        var animdir = _Shooter.transform.InverseTransformDirection(dir);
        var isfacingmovedirection = Vector3.Dot(dir, _Shooter.transform.forward) > 0.5f;

        _Shooter.anim.SetFloat("Horizontal", isfacingmovedirection ? animdir.x : 0, .5f, Time.deltaTime);
        _Shooter.anim.SetFloat("Vertical", isfacingmovedirection ? animdir.z : 0, .5f, Time.deltaTime);*/


        float distanceToPlayer = Vector3.Distance(_Shooter.transform.position, _Shooter.player.position);
       
        if (_Shooter.canSeePlayer)
        {

        }

        if (distanceToPlayer < _Shooter.attackRange)
        _Shooter.playerInAttackRange = true;

    }

    public void OnExit()
    {
        Debug.Log("Chase OnExit");
        _Shooter.anim.SetBool("Walk", false);
    }
}
