using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    E_Shooter _shooter;
    StateMachine _FSM;

    public Chase( E_Shooter shooter, StateMachine fSM)
    {

        _shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        Debug.Log("Chase OnEnter");
        _shooter.velocity = Vector3.zero;
        _shooter.alreadyAttacked = false;
    }

    public void Tick()
    {

        // Siempre sigue al jugador directamente
        Vector3 targetPos = _shooter.player.position;

        // Calcular dirección usando Seek y evitar obstáculos
        Vector3 steer = _shooter.Seek(targetPos) + _shooter.ObstacleAvoidance();
        _shooter.AddForce(steer);

        Vector3 velocity = _shooter.rb.velocity;
        Vector3 flatVel = new Vector3(velocity.x, 0, velocity.z);

        if (flatVel.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(flatVel.normalized);
            _shooter.transform.rotation = Quaternion.Slerp(_shooter.transform.rotation, rot, Time.deltaTime * 8f);
        }

        Vector3 local = _shooter.transform.InverseTransformDirection(flatVel.normalized);
        _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

        // Si está en rango de ataque, marcarlo
        float dist = Vector3.Distance(_shooter.transform.position, _shooter.player.position);
        if (dist <= _shooter.attackRange)
        {
            _shooter.playerInAttackRange = true;
        }
    }

    public void OnExit()
    {
        Debug.Log("Chase OnExit");
        _shooter.lastposition = GameManager.instance.thisIsPlayer.position;
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }
}
