using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroFlank : IState
{
    private Rigidbody _rb;
    private E_Shooter _shooter;
    private StateMachine _fsm;

    private Vector3 _flankDirection;
    private float _duration = 9f;
    private float _timer = 0f;

    private float _flankSpeed = 4f;

    public MicroFlank( E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _timer = 0f;
        
        // Decide una dirección lateral o ligeramente diagonal
        Vector3 toPlayer = (_shooter.player.position - _shooter.transform.position).normalized;
        Vector3 lateral = Vector3.Cross(Vector3.up, toPlayer).normalized;
        _flankDirection = Quaternion.Euler(0, Random.Range(-30f, 30f), 0) * lateral;

        Debug.Log($"[{_shooter.name}] Iniciando micro flanqueo hacia {_flankDirection}");
    }

    public void Tick()
    {
        _timer += Time.deltaTime;
        _shooter.rb.MovePosition(_rb.position + _flankDirection * _flankSpeed * Time.deltaTime);

        // Actualiza animaciones si es necesario
        Vector3 localDir = _shooter.transform.InverseTransformDirection(_flankDirection);
        _shooter.anim.SetFloat("Horizontal", localDir.x, 0.2f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", localDir.z, 0.2f, Time.deltaTime);

        if (_timer >= _duration)
        {
            _shooter.finishedMicroFlank = true;
        }
    }

    public void OnExit()
    {
        _shooter.anim.SetFloat("Horizontal", 0);
        _shooter.anim.SetFloat("Vertical", 0);
    }
}
