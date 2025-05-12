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
        Debug.Log($"[{_shooter.name}] Enter Chase");
        // Limpiamos cualquier velocidad residual
        _shooter.velocity = Vector3.zero;
        // Aseguramos que no esté marcado como atacado
        _shooter.alreadyAttacked = false;
    }

    public void Tick()
    {

        // 1) Calculamos el steering hacia el jugador
        var steer = _shooter.Seek(_shooter.player.position)
                  + _shooter.ObstacleAvoidance();

        _shooter.AddForce(steer);

        // 2) Movemos según la velocity resultante
        Vector3 movement = _shooter.velocity * Time.deltaTime;
        _shooter.transform.position += movement;

        // 3) Rotación suave hacia la dirección de movimiento real
        if (movement.sqrMagnitude > 0.001f)
        {
            Vector3 flat = new Vector3(movement.x, 0, movement.z).normalized;
            var look = Quaternion.LookRotation(flat);
            _shooter.transform.rotation = Quaternion.Slerp(
                _shooter.transform.rotation, look, Time.deltaTime * 8f);
        }

        // 4) Animaciones 2D blend-tree
        Vector3 localDir = _shooter.transform.InverseTransformDirection(
            _shooter.velocity.normalized);
        _shooter.anim.SetFloat("Horizontal", localDir.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", localDir.z, 0.1f, Time.deltaTime);

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
        Debug.Log($"[{_shooter.name}] Exit Chase");
        // Frenar animación de correr
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }
}
