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
        strafeTimer = 0f;
        moveLeft = Random.Range(0f, 1f) > 0.5f;
    }

    public void Tick()
    {
        strafeTimer += Time.deltaTime;

        if (strafeTimer >= strafeDuration)
        {
            // Cambia la dirección después del intervalo
            moveLeft = !moveLeft;
            strafeTimer = 0f;
        }

        // Movimiento lateral
        Vector3 direction = moveLeft ? -_Shooter.transform.right : _Shooter.transform.right;
        _Shooter.transform.position += direction * _Shooter.strafeSpeed * Time.deltaTime;

    }


    public void OnExit()
    {

    }
}
