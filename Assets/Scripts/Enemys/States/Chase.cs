using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : IState
{
    E_Shooter _shooter;
    StateMachine _FSM;

    private static readonly Vector3[] FormationOffsets = {
        Vector3.zero,
        new Vector3( 2f, 0,  1f),
        new Vector3(-2f, 0,  1f),
        new Vector3( 0f, 0, -2f),
        new Vector3( 3f, 0,  1f),
        new Vector3(-3f, 0,  1f)
    };

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

        // 1) Siempre sigue al jugador directamente
        Vector3 targetPos = _shooter.player.position;

        // 2) Calcular dirección usando Seek y evitar obstáculos
        Vector3 steer = _shooter.Seek(targetPos) + _shooter.ObstacleAvoidance();
        _shooter.AddForce(steer);

        // 3) Movimiento físico
        Vector3 delta = _shooter.velocity * Time.deltaTime;
        _shooter.rb.MovePosition(_shooter.rb.position + delta);

        // 4) Rotación y animación
        if (delta.sqrMagnitude > 0.001f)
        {
            Vector3 flat = new Vector3(delta.x, 0, delta.z).normalized;
            Quaternion rot = Quaternion.LookRotation(flat);
            _shooter.transform.rotation = Quaternion.Slerp(
                _shooter.transform.rotation, rot, Time.deltaTime * 8f);
        }

        Vector3 local = _shooter.transform.InverseTransformDirection(delta.normalized);
        _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

        // 5) Si está en rango de ataque, marcarlo
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
