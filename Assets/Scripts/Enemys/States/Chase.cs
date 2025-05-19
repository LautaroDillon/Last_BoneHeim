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
       // Debug.Log($"[{_shooter.name}] Enter Chase");
        // Limpiamos cualquier velocidad residual
        _shooter.velocity = Vector3.zero;
        // Aseguramos que no esté marcado como atacado
        _shooter.alreadyAttacked = false;
    }

    public void Tick()
    {

        Vector3 targetPos;

        if (_shooter.isLeader)
        {
            // líder va directo al jugador
            targetPos = _shooter.player.position;
        }
        else
        {
            // subordinado: sigue la posición del líder + su offset
            var leader = GameManager.instance.GetGroupLeader(_shooter.zoneId);
            if (leader == null)
            {
                targetPos = _shooter.player.position; // fallback
            }
            else
            {
                // Elegimos offset según su groupIndex
                int idx = _shooter.groupIndex % FormationOffsets.Length;
                Vector3 worldOffset = leader.transform.TransformDirection(FormationOffsets[idx]);
                targetPos = leader.transform.position + worldOffset;
            }
        }

        // Steering hacia targetPos
        var steer = _shooter.Seek(targetPos)
                  + _shooter.ObstacleAvoidance();
        _shooter.AddForce(steer);

        // Mover con física
        Vector3 delta = _shooter.velocity * Time.deltaTime;
        _shooter.rb.MovePosition(_shooter.rb.position + delta);

        // Rotar hacia movimiento real
        if (delta.sqrMagnitude > 0.001f)
        {
            Vector3 flat = new Vector3(delta.x, 0, delta.z).normalized;
            Quaternion rot = Quaternion.LookRotation(flat);
            _shooter.transform.rotation = Quaternion.Slerp(
                _shooter.transform.rotation, rot, Time.deltaTime * 8f);
        }

        // Animaciones blend-tree
        Vector3 local = _shooter.transform.InverseTransformDirection(delta.normalized);
        _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

        // 5) Si entro en rango de ataque, cambio a AttackState
        float dist = Vector3.Distance(
            _shooter.transform.position,
            _shooter.player.position);
        if (dist <= _shooter.attackRange)
        {
           _shooter.playerInAttackRange = true;
        }
    }

    public void OnExit()
    {
        //Debug.Log($"[{_shooter.name}] Exit Chase");
        // Frenar animación de correr
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }
}
