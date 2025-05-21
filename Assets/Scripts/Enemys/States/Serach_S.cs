using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Serach_S : IState
{
    
    E_Shooter _shooter;
    StateMachine _fsm;

    private Vector3 _targetSearchPoint;
    private bool _movingToLastPosition = true;
    private bool _movingToVisibleNode = false;

    public Serach_S( E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Search OnEnter");
        _movingToLastPosition = true;
        _movingToVisibleNode = false;
        _targetSearchPoint = _shooter.lastposition;
    }

    public void Tick()
    {
        Vector3 dir = (_targetSearchPoint - _shooter.transform.position).normalized;
        Vector3 move = dir * _shooter.moveSpeed * Time.deltaTime;

        _shooter.rb.MovePosition(_shooter.rb.position + move);

        // Rotación y animación
        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            _shooter.transform.rotation = Quaternion.Slerp(_shooter.transform.rotation, rot, Time.deltaTime * 5f);
        }

        Vector3 localDir = _shooter.transform.InverseTransformDirection(dir);
        _shooter.anim.SetFloat("Horizontal", localDir.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", localDir.z, 0.1f, Time.deltaTime);

        if (Vector3.Distance(_shooter.transform.position, _targetSearchPoint) < _shooter.nodeReachDistance)
        {
            if (_movingToLastPosition)
            {
                _movingToLastPosition = false;
                TryFindVisibleNode();
            }
            else if (_movingToVisibleNode)
            {
                Debug.Log("Search finalizado.");
                _shooter.lastposition = Vector3.zero;
                // Podés forzar una transición acá si querés
                _shooter.isPatrolling = false;          
            }
        }
    }

    public void OnExit()
    {
        _shooter.lastposition = new Vector3(0,0,0);
        if (!_shooter.canSeePlayer && !_shooter.otherSeenPlayer)
            _shooter.isPatrolling = true;
        if (_shooter.canSeePlayer)
            
        Debug.Log("Search OnExit");
    }

    private void TryFindVisibleNode()
    {
        var allNodes = ManagerNode.Instance.GetNodesInZone(_shooter.zoneId);
        List<NodePathfinding> visibleNodes = new();

        foreach (var node in allNodes)
        {
            Vector3 dirToNode = node.transform.position - _shooter.transform.position;
            float dist = dirToNode.magnitude;

            if (!Physics.Raycast(_shooter.transform.position, dirToNode.normalized, dist, _shooter.obstructionMask))
            {
                visibleNodes.Add(node);
            }
        }

        if (visibleNodes.Count > 0)
        {
            NodePathfinding chosen = visibleNodes[0];
            float closest = Vector3.Distance(_shooter.transform.position, chosen.transform.position);

            foreach (var node in visibleNodes)
            {
                float dist = Vector3.Distance(_shooter.transform.position, node.transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    chosen = node;
                }
            }

            // actualizar el puntodestino
            _targetSearchPoint = chosen.transform.position;
            _movingToVisibleNode = true;
        }
        else
        {
            Debug.LogWarning("No se encontraron nodos visibles desde la posición actual.");
            _shooter.isPatrolling = false; // fallback
        }
    }
}
