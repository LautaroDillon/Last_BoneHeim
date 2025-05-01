using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Strafe : IState
{
    NavMeshAgent _agent;
    E_Shooter _Shooter;
    StateMachine _fsm;

    private float strafeTimer = 0f;
    private float strafeDuration = 2f;  // Duración del strafe en segundos
    private bool moveLeft = true;

    public Strafe(NavMeshAgent agent,E_Shooter shooter,StateMachine fsm)
    {
        _agent = agent;
        _Shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Strafe OnEnter");
        strafeTimer = 0f;
        _agent.speed = 1.5f;
        moveLeft = Random.Range(0f, 1f) > 0.5f;
    }

    public void Tick()
    {


        var dir = (_agent.steeringTarget - _Shooter.transform.position).normalized;
        var animdir = _Shooter.transform.InverseTransformDirection(dir);
        var isfacingmovedirection = Vector3.Dot(dir, _Shooter.transform.forward) > 0.5f;

        _Shooter.anim.SetFloat("Horizontal", isfacingmovedirection ? animdir.x : 0, .5f, Time.deltaTime);
        _Shooter.anim.SetFloat("Vertical", isfacingmovedirection ? animdir.z : 0, .5f, Time.deltaTime);

        strafeTimer += Time.deltaTime;

        if (strafeTimer >= strafeDuration)
        {
            // Cambia la dirección después del intervalo
            moveLeft = !moveLeft;
            strafeTimer = 0f;
            _Shooter.alreadyAttacked = false; // Permitir el ataque después de un strafe
        }

        // Movimiento lateral
        Vector3 direction = moveLeft ? -_Shooter.transform.right : _Shooter.transform.right;
        _Shooter.transform.position += direction * _Shooter.strafeSpeed * Time.deltaTime;

    }


    public void OnExit()
    {
        Debug.Log("Strafe OnExit");
    }
}
