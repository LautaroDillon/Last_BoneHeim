using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Strafe : IState
{
    private E_Shooter _shooter;
    private StateMachine _fsm;

    private float _timer;
    private float _duration;
    private float _speed;          // velocidad de strafe
    private int _direction;      // +1 = derecha, -1 = izquierda
    private float _noiseOffset;

    // Parámetros ajustables
    private const float MinDuration = 0.8f;
    private const float MaxDuration = 1.5f;
    private const float MinSpeedMult = 1.0f;
    private const float MaxSpeedMult = 1.4f;
    private const float JitterAmp = 0.5f;
    private const float ForwardBias = 0.3f;

    public Strafe(E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        // Duración y velocidad aleatorias
        _duration = Random.Range(MinDuration, MaxDuration);
        _speed = _shooter.strafeSpeed * Random.Range(MinSpeedMult, MaxSpeedMult);
        _direction = Random.value < .5f ? 1 : -1;
        _timer = 0f;
        _noiseOffset = Random.value * 100f;

        // Trigger de animación de strafe
        _shooter.anim.SetTrigger("Strafe");
    }

    public void Tick()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            // terminamos el strafe
            _shooter.alreadyAttacked = false;
            return;
        }

        // Dirección a jugador
        Vector3 toPlayer = (_shooter.player.position - _shooter.transform.position).normalized;

        // Lateral puro
        Vector3 lateral = Vector3.Cross(Vector3.up, toPlayer) * _direction;

        // Un poco de forward/back bias
        Vector3 forwardBack = toPlayer * Random.Range(-ForwardBias, ForwardBias);

        // Jitter suave con Perlin
        float noise = (Mathf.PerlinNoise(Time.time * 1.5f, _noiseOffset) - 0.5f) * 2f;
        Vector3 jitter = lateral * (noise * JitterAmp);

        // Vector final de movimiento
        Vector3 moveDir = (lateral + forwardBack + jitter).normalized;

        //  Aplicar movimiento
        Vector3 delta = moveDir * _speed * Time.deltaTime;
        _shooter.transform.position += delta;

        // Rotación suave hacia la propia dirección de movimiento
        if (delta.sqrMagnitude > 0.001f)
        {
            Vector3 flat = new Vector3(delta.x, 0, delta.z).normalized;
            Quaternion look = Quaternion.LookRotation(flat);
            _shooter.transform.rotation = Quaternion.Slerp(
                _shooter.transform.rotation, look, Time.deltaTime * 8f);
        }

        Vector3 local = _shooter.transform.InverseTransformDirection(moveDir);
        _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);
    }

    public void OnExit()
    {
        // Limpiar animaciones
        _shooter.anim.ResetTrigger("Strafe");
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }


}
