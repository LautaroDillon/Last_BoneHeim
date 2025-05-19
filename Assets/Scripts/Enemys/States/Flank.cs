using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flank : IState
{
    private E_Shooter _shooter;
    private StateMachine _fsm;
    private NodePathfinding _targetNode;
    private float _flankDistance = 5f;
    private bool _hasPath;

    public Flank(E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Flank OnEnter");
        _hasPath = false;

        Vector3 directionToPlayer = (_shooter.player.position - _shooter.transform.position).normalized;
        Vector3 flankDirection = Vector3.Cross(Vector3.up, directionToPlayer); // 90° hacia un lado
        if (Random.value > 0.5f)
            flankDirection *= -1; // a veces izquierda, a veces derecha

        Vector3 flankPosition = _shooter.player.position + flankDirection * _flankDistance;

        NodePathfinding start = ManagerNode.Instance.GetClosestNode(_shooter.transform.position, _shooter.zoneId);
        NodePathfinding end = ManagerNode.Instance.GetClosestNode(flankPosition, _shooter.zoneId);

        if (start != null && end != null)
        {
            _shooter.path = ManagerNode.Instance.FindPath(start, end);
            _shooter.pathIndex = 0;
            _hasPath = _shooter.path != null && _shooter.path.Count > 0;

            Debug.Log($"[Flank] Flanqueando desde {start.name} a {end.name}");
        }
    }

    public void Tick()
    {
        if (!_hasPath || _shooter.pathIndex >= _shooter.path.Count)
        {
            _fsm.SetState(new Chase(_shooter, _fsm)); // volver a perseguir si terminó
            return;
        }

        var node = _shooter.path[_shooter.pathIndex];
        Vector3 dir = (node.transform.position - _shooter.transform.position).normalized;
        Vector3 move = dir * _shooter.moveSpeed * Time.deltaTime;

        // rotación y animación
        _shooter.transform.rotation = Quaternion.Slerp(_shooter.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        Vector3 local = _shooter.transform.InverseTransformDirection(dir);
        _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

        _shooter.transform.position += move;

        if (Vector3.Distance(_shooter.transform.position, node.transform.position) < _shooter.nodeReachDistance)
            _shooter.pathIndex++;
    }

    public void OnExit()
    {
        _shooter.flankTimer = 0f;
        _shooter.anim.SetFloat("Horizontal", 0);
        _shooter.anim.SetFloat("Vertical", 0);
    }
}
