using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Strafe : IState
{
    private E_Shooter _shooter;
    private StateMachine _fsm;

    private float _timer;
    private readonly float _duration = 1.2f;      // tiempo que dura el strafe
    private Vector3 _targetPos;
    private bool _toRight;

    public Strafe( E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _timer = 0f;
        // Elegir aleatoriamente si strafea a la derecha o izquierda
        _toRight = Random.value < 0.5f;

        // Calculamos una posición objetivo relativa al jugador:
        Vector3 toPlayer = (_shooter.player.position - _shooter.transform.position).normalized;
        // Vector lateral:
        Vector3 strafeDir = _toRight
            ? Vector3.Cross(Vector3.up, toPlayer)  // derecha
            : Vector3.Cross(toPlayer, Vector3.up); // izquierda

        // Distancia lateral entre 2 y 4 metros
        float latDist = Random.Range(2f, 4f);
        // Queremos mantenernos dentro del rango de ataque:
        float forwardDist = Mathf.Clamp(
            Vector3.Distance(_shooter.transform.position, _shooter.player.position),
            _shooter.attackRange * 0.8f,
            _shooter.attackRange * 1.1f
        );

        // Construir targetPos alrededor del jugador:
        _targetPos = _shooter.player.position
                   - toPlayer * forwardDist
                   + strafeDir * latDist;

        // Trigger de anim si lo necesitás
       //_shooter.anim.SetTrigger("Strafe");
    }

    public void Tick()
    {
        _timer += Time.deltaTime;

        // Movernos hacia _targetPos con steering u orientación directa:
        Vector3 dir = (_targetPos - _shooter.transform.position).normalized;
        Vector3 movement = dir * _shooter.strafeSpeed * Time.deltaTime;
        _shooter.transform.position += movement;

        // Rotar para mirar en la dirección de movimiento
        if (movement.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(movement.normalized);
            _shooter.transform.rotation = Quaternion.Slerp(
                _shooter.transform.rotation,
                lookRot,
                Time.deltaTime * 8f
            );
        }

        // Animación: Horizontal/Vertical según movimiento local
        Vector3 local = _shooter.transform.InverseTransformDirection(dir);
        _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

        // Si se acaba el tiempo o llega al target:
        if (_timer >= _duration
         || Vector3.Distance(_shooter.transform.position, _targetPos) < 0.5f)
        {
            // marcamos que ya no está atacando, listo para volver al estado anterior
            _shooter.alreadyAttacked = false;           
        }
    }

    public void OnExit()
    {
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }

   
}
