using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : IState
{
    E_Shooter _shooter;
    StateMachine _FSM;
    private float _waitTimer;
    private float _waitDuration = 5f;

    public Patrol(E_Shooter shooter, StateMachine fSM)
    {
        _shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        _shooter.isPatrolling = true;
        _waitTimer = 0f;
        ChooseRandomNodeInZone();
    }

    public void Tick()
    {
        if (_shooter.path != null && _shooter.pathIndex < _shooter.path.Count)
        {
            var node = _shooter.path[_shooter.pathIndex];
            Vector3 dir = (node.transform.position - _shooter.transform.position).normalized;

            Vector3 force = dir * _shooter.maxForce;
            _shooter.rb.AddForce(force, ForceMode.Acceleration);

            if (_shooter.rb.velocity.magnitude > _shooter.maxSpeed)
            {
                _shooter.rb.velocity = _shooter.rb.velocity.normalized * _shooter.maxSpeed;
            }

            if (force.sqrMagnitude > 0.001f)
            {
                Vector3 flat = new Vector3(force.x, 0, force.z).normalized;
                Quaternion rot = Quaternion.LookRotation(flat);
                _shooter.transform.rotation = Quaternion.Slerp(
                    _shooter.transform.rotation, rot, Time.deltaTime * 5f);
            }

            Vector3 local = _shooter.transform.InverseTransformDirection(dir);
            _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
            _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

            if (Vector3.Distance(_shooter.transform.position, node.transform.position) < _shooter.nodeReachDistance)
            {
                _shooter.pathIndex++;
            }
        }
        else
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _waitDuration)
            {
                _waitTimer = 0f;
                ChooseRandomNodeInZone();
                _shooter.isPatrolling = true;
            }

            _shooter.anim.SetFloat("Horizontal", 0f);
            _shooter.anim.SetFloat("Vertical", 0f);
        }
    }

    public void OnExit()
    {
        _shooter.isPatrolling = false;
        _shooter.rb.velocity = Vector3.zero;
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }

    private void ChooseRandomNodeInZone()
    {
        var zoneNodes = ManagerNode.Instance.GetNodesInZone(_shooter.zoneId);
        if (zoneNodes == null || zoneNodes.Count < 2)
            return;

        var start = ManagerNode.Instance.GetClosestNode(_shooter.transform.position, _shooter.zoneId);
        if (start == null)
            return;

        zoneNodes.Remove(start);
        if (zoneNodes.Count == 0)
            return;

        var dest = zoneNodes[Random.Range(0, zoneNodes.Count)];

        _shooter.path = ManagerNode.Instance.FindPath(start, dest);
        _shooter.pathIndex = 0;

        for (int i = 0; i < _shooter.path.Count - 1; i++)
        {
            Debug.DrawLine(_shooter.path[i].transform.position, _shooter.path[i + 1].transform.position, Color.green, 2f);
        }
    }
}
