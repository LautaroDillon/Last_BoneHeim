using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : IState
{

    E_Shooter _shooter;
    StateMachine _FSM;
    private List<NodePathfinding> _path;
    private int _pathIndex;

    // Opcionales: definir un cooldown antes de cambiar de nodo
    private float _waitTimer;
    private float _waitDuration = 1f;

    public Patrol( E_Shooter shooter, StateMachine fSM)
    {
        _shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        Debug.Log("Patrol: OnEnter");
        _waitTimer = 0f;
        _path = null;
        _pathIndex = 0;
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
        _shooter.isPatrolling = true;
        ChooseRandomVisibleNodePath();
    }

    public void Tick()
    {
        if (_path != null && _pathIndex < _path.Count)
        {
            Vector3 targetPos = _path[_pathIndex].transform.position;
            Vector3 dir = (targetPos - _shooter.transform.position).normalized;

            // Movimiento directo hacia el nodo
            _shooter.transform.position += dir * _shooter.WalkSpeed * Time.deltaTime;

            // Rotación suave
            if (dir.magnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                _shooter.transform.rotation = Quaternion.Slerp(_shooter.transform.rotation, lookRot, Time.deltaTime * 5f);
            }

            // Animación si usás Horizontal / Vertical
            Vector3 localDir = _shooter.transform.InverseTransformDirection(dir);
            _shooter.anim.SetFloat("Horizontal", localDir.x, 0.1f, Time.deltaTime);
            _shooter.anim.SetFloat("Vertical", localDir.z, 0.1f, Time.deltaTime);

            if (Vector3.Distance(_shooter.transform.position, targetPos) < _shooter.arriveRadius)
            {
                _pathIndex++;
            }
        }
        else
        {
            _shooter.anim.SetFloat("Horizontal", 0f, 0.1f, Time.deltaTime);
            _shooter.anim.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);

            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _waitDuration)
            {
                _waitTimer = 0f;
                _path = null;
                _pathIndex = 0;
                ChooseRandomVisibleNodePath();
            }

        }
    }

    public void OnExit()
    {
        Debug.Log("Patrol: OnExit");
        // Limpiar fuerza/velocidad
        _shooter.isPatrolling = false;
        _shooter.anim.SetFloat("Horizontal", 0f);
        _shooter.anim.SetFloat("Vertical", 0f);
        _shooter.velocity = Vector3.zero;
        _path = null;
    }

    private void ChooseRandomVisibleNodePath()
    {
        _waitTimer = 0f;

        // 1) Conseguir todos los nodos
        var allNodes = ManagerNode.Instance.nodes; // asume array público
        var visible = new List<NodePathfinding>();

        // 2) Filtrar solo los visibles: sin obstáculos entre shooter y nodo
        foreach (var node in allNodes)
        {
            Vector3 dir = (node.transform.position - _shooter.transform.position).normalized;
            float dist = Vector3.Distance(_shooter.transform.position, node.transform.position);

            // raycast contra capa de obstáculos
            if (!Physics.Raycast(_shooter.transform.position, dir, dist, _shooter.obstructionMask))
                visible.Add(node);
        }

        if (visible.Count == 0)
        {
            // fallback: volver a intento más tarde
            return;
        }

        // 3) Elegir uno al azar
        var destNode = visible[Random.Range(0, visible.Count)];

        // 4) Calcular ruta con tu A* Theta*
        var startNode = ManagerNode.Instance.NodeProx(_shooter.transform.position);
        _path = _shooter.CalculateThetaStar(startNode, destNode);

        _pathIndex = 0;
    }
}
