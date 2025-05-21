using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : IState
{

    E_Shooter _shooter;
    StateMachine _FSM;
    private float _waitTimer;
    private float _waitDuration = 5;
    public Patrol( E_Shooter shooter, StateMachine fSM)
    {
        _shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        _shooter.isPatrolling = true;
        Debug.Log($"[{_shooter.name}] Patrol OnEnter (zona {_shooter.zoneId})");
        _waitTimer = 0f;
        ChooseRandomNodeInZone();
    }

    public void Tick()
    {
        if (_shooter.path != null && _shooter.pathIndex < _shooter.path.Count)
        {
            var node = _shooter.path[_shooter.pathIndex];
            Vector3 dir = (node.transform.position - _shooter.transform.position).normalized;

            // Movimiento real
            Vector3 movement = dir * _shooter.moveSpeed * Time.deltaTime;
            _shooter.rb.MovePosition(_shooter.rb.position + movement);

            // Rotación hacia la dirección de movimiento
            if (movement.sqrMagnitude > 0.001f)
            {
                Vector3 flat = new Vector3(movement.x, 0, movement.z).normalized;
                Quaternion rot = Quaternion.LookRotation(flat);
                _shooter.transform.rotation = Quaternion.Slerp(
                    _shooter.transform.rotation, rot, Time.deltaTime * 5f);
            }

            // Animaciones
            Vector3 local = _shooter.transform.InverseTransformDirection(dir);
            _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
            _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

            // Aplicar movimiento
            //_shooter.transform.position += movement;

            // Avanzar al siguiente nodo si llegamos
            if (Vector3.Distance(_shooter.transform.position, node.transform.position)
                < _shooter.nodeReachDistance)
            {
                _shooter.pathIndex++;
            }
        }
        else
        {
            _shooter.isPatrolling = false;
        }
    }

    public void OnExit()
    {
        //Debug.Log($"[{_shooter.name}] Patrol OnExit");
        // Frenar anim y movimiento
        _shooter.isPatrolling = false;
        _shooter.rb.velocity = Vector3.zero;
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
    }

    private void ChooseRandomNodeInZone()
    {
        // 1) Obtener lista de nodos de la zona
        var zoneNodes = ManagerNode.Instance.GetNodesInZone(_shooter.zoneId);
        if (zoneNodes == null || zoneNodes.Count == 0)
        {
            return;
        }

        // 2) Nodo de inicio más cercano
        var start = ManagerNode.Instance.GetClosestNode(_shooter.transform.position, _shooter.zoneId);
        if (start == null)
        {
            return;
        }

        // 3) Nodo destino aleatorio distinto al inicio
        NodePathfinding dest = start;
        int tries = 0;
        while (dest == start && tries < 10)
        {
            dest = zoneNodes[Random.Range(0, zoneNodes.Count)];
            tries++;
        }

        // 4) Calcular A y asignar ruta
        _shooter.path = ManagerNode.Instance.FindPath(start, dest);
        _shooter.pathIndex = 0;
    }
}
