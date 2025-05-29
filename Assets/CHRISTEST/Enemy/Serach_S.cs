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
    private bool _waitingAtLastPosition = false;
    private bool _movingToVisibleNode = false;

    private float _waitDuration = 2f;
    private float _waitTimer = 0f;

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
        _waitingAtLastPosition = false;
        _waitTimer = 0f;

        if (Physics.Raycast(_shooter.lastposition + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, _shooter.whatIsGround))
        {
            _targetSearchPoint = hit.point;
            Debug.DrawLine(_targetSearchPoint + Vector3.up * 2f, hit.point, Color.green, 2f);
        }
        else
        {
            // Si falla, usar la posición tal como está
            _targetSearchPoint = _shooter.lastposition;
            Debug.LogWarning("No se pudo ajustar la posición del player al suelo");
        }
    }

    public void Tick()
    {

        if (_movingToLastPosition)
        {
            MoverHacia(_targetSearchPoint);

            if (Vector3.Distance(_shooter.transform.position, _targetSearchPoint) < _shooter.nodeReachDistance)
            {
                _movingToLastPosition = false;
                _waitingAtLastPosition = true;
                _shooter.velocity = Vector3.zero;
                _shooter.anim.SetFloat("Horizontal", 0);
                _shooter.anim.SetFloat("Vertical", 0);
            }
        }
        else if (_waitingAtLastPosition)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _waitDuration)
            {
                _waitingAtLastPosition = false;
                TryFindVisibleNode();
            }
        }
        else if (_movingToVisibleNode)
        {
            MoverHacia(_targetSearchPoint);
            //pregunta si el shooter ha llegado al nodo
            if (Vector3.Distance(_shooter.transform.position, _targetSearchPoint) < _shooter.nodeReachDistance)
            {
                Debug.Log("Search finalizado.");
                _shooter.lastposition = Vector3.zero;
                _shooter.isPatrolling = false;
                _shooter.finishsearching = true;
            }
        }

        // Si el jugador aparece de nuevo
        float distance = Vector3.Distance(_shooter.transform.position, _shooter.player.position);
        if (distance < _shooter.attackRange)
        {
            _shooter.playerInAttackRange = true;
        }

    }

    public void OnExit()
    {
        _shooter.lastposition = new Vector3(0,0,0);
        /*if (!_shooter.canSeePlayer && !_shooter.otherSeenPlayer)
            _shooter.isPatrolling = true;*/
        //if (_shooter.canSeePlayer)
        _shooter.finishsearching = true;

        Debug.Log("Search OnExit");
    }

    private void MoverHacia(Vector3 destino)
    {
        Vector3 steering = _shooter.Seek(destino) + _shooter.ObstacleAvoidance();
        _shooter.AddForce(steering);

        Vector3 delta = _shooter.velocity * Time.deltaTime;

        if (delta.sqrMagnitude > 0.001f)
        {
            Vector3 flat = new Vector3(delta.x, 0, delta.z).normalized;
            Quaternion rot = Quaternion.LookRotation(flat);
            _shooter.transform.rotation = Quaternion.Slerp(_shooter.transform.rotation, rot, Time.deltaTime * 5f);
        }

        Vector3 localDir = _shooter.transform.InverseTransformDirection(delta.normalized);
        _shooter.anim.SetFloat("Horizontal", localDir.x, 0.1f, Time.deltaTime);
        _shooter.anim.SetFloat("Vertical", localDir.z, 0.1f, Time.deltaTime);
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

            _targetSearchPoint = chosen.transform.position;
            _movingToVisibleNode = true;
        }
        else
        {
            Debug.LogWarning("No se encontraron nodos visibles desde la posición actual.");
            _shooter.finishsearching = true;
            _shooter.isPatrolling = false;
        }
    }
}
