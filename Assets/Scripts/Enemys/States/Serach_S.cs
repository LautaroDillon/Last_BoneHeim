using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Serach_S : IState
{
    
    E_Shooter _shooter;
    StateMachine _fsm;
    private float _searchDuration = 3f;
    private float _searchTimer;

    private float _waitTimer;
    private float _waitDuration = 2f;

    private bool _movingToNewPoint = false;
    private Vector3 _targetSearchPoint;

    public Serach_S( E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Search OnEnter");
        _searchTimer = 0f;
        _waitTimer = 0f;
        _movingToNewPoint = false;
        _targetSearchPoint = _shooter.lastposition;
    }

    public void Tick()
    {

        if (_movingToNewPoint)
        {
            Vector3 dir = (_targetSearchPoint - _shooter.transform.position).normalized;
            Vector3 move = dir * _shooter.moveSpeed * Time.deltaTime;

            _shooter.rb.MovePosition(_shooter.rb.position + move);

            // Animación y rotación
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                _shooter.transform.rotation = Quaternion.Slerp(
                    _shooter.transform.rotation, rot, Time.deltaTime * 5f
                );
            }

            Vector3 localDir = _shooter.transform.InverseTransformDirection(dir);
            _shooter.anim.SetFloat("Horizontal", localDir.x, 0.1f, Time.deltaTime);
            _shooter.anim.SetFloat("Vertical", localDir.z, 0.1f, Time.deltaTime);

            if (Vector3.Distance(_shooter.transform.position, _targetSearchPoint) < _shooter.nodeReachDistance)
            {
                _movingToNewPoint = false;
                _shooter.lastposition = Vector3.zero;
            }
        }
    }

    public void OnExit()
    {
        _shooter.lastposition = new Vector3(0,0,0);
        Debug.Log("Search OnExit");
    }
}
