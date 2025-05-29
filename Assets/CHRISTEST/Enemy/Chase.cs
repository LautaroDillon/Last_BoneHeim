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

        TryStepUp(targetPos);

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

    private void TryStepUp(Vector3 dir)
    {
        if (_shooter.col == null) return;

        CapsuleCollider col = _shooter.col;

        // Origen desde la base del collider (ligeramente elevado para no tocar el suelo directamente)
        Vector3 rayOrigin = _shooter.rayFoot.position;
        float rayDistance = 1f;

        // Visualización del raycast
        Debug.DrawRay(rayOrigin, dir.normalized * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayDistance, ~LayerMask.GetMask("Enemy")))
        {
            Vector3 normal = hit.normal;
            float angle = Vector3.Angle(normal, Vector3.up);

            if (angle > 40f && angle < 80f)
            {
                // Aplica impulso vertical leve para subir la rampa/escalón
                Vector3 upwardBoost = Vector3.up * 4f + dir.normalized * 1f;
                _shooter.rb.AddForce(upwardBoost, ForceMode.VelocityChange);
            }
        }
    }
}
