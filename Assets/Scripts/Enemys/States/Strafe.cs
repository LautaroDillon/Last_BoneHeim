using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Strafe : IState
{
    private E_Shooter _shooter;
    private StateMachine _fsm;

    private float _waitDuration = 2f;
    private float _waitTimer = 0f;

    bool gotoplayer;
    float count;

    public Strafe(E_Shooter shooter, StateMachine fsm)
    {
        _shooter = shooter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        ChooseRandomNodeInZone();
        count = 0;
        gotoplayer = false;
    }

    public void Tick()
    {

        if(_waitTimer < _waitDuration)
        {
            _waitTimer += Time.deltaTime;
        }
        else if (_waitTimer >= _waitDuration)
        {
            getrandomNum();
            _shooter.anim.SetFloat("Horizontal", 0);
            _shooter.anim.SetFloat("Vertical", 0);
            _waitTimer = 0f;
        }

        // Comprobar si el jugador está en rango de ataque
        float distance = Vector3.Distance(_shooter.transform.position, _shooter.player.position);
        if (distance < _shooter.attackRange)
        {
            _shooter.playerInAttackRange = true;
        }


        if (_shooter.path != null && _shooter.pathIndex < _shooter.path.Count && _waitTimer < _waitDuration)
        {
            var node = _shooter.path[_shooter.pathIndex];
            Vector3 dir = (node.transform.position - _shooter.transform.position).normalized;

            // Movimiento real
            Vector3 force = dir * _shooter.maxForce;
            _shooter.rb.AddForce(force, ForceMode.Acceleration);

            // Rotación hacia la dirección de movimiento
            if (force.sqrMagnitude > 0.001f)
            {
                Vector3 flat = new Vector3(force.x, 0, force.z).normalized;
                Quaternion rot = Quaternion.LookRotation(flat);
                _shooter.transform.rotation = Quaternion.Slerp(
                    _shooter.transform.rotation, rot, Time.deltaTime * 5f);
            }

            // Animaciones
            Vector3 local = _shooter.transform.InverseTransformDirection(dir);
            _shooter.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
            _shooter.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

            // Avanzar al siguiente nodo si llegamos
            if (Vector3.Distance(_shooter.transform.position, node.transform.position)
                < _shooter.nodeReachDistance)
            {
                _shooter.pathIndex++;
            }

        }
        else if (_shooter.path == null)
        {
            // Si no hay más nodos, elegir uno nuevo y
            // preguntar si el enemigo busca al jugador
            ChooseRandomNodeInZone();
            getrandomNum();
            return;
        }

        // si gotoplayer es verdadero, el enemigo se mueve hacia el jugador
        if (gotoplayer)
        {
            _shooter.voyaPlayer = true;
            gotoplayer = false;
        }

    }

    public void OnExit()
    {
        _shooter.path = null;
    }

    private void ChooseRandomNodeInZone()
    {
        // Obtener lista de nodos de la zona
        var zoneNodes = ManagerNode.Instance.GetNodesInZone(_shooter.zoneId);
        if (zoneNodes == null || zoneNodes.Count == 0)
        {
            return;
        }

        // Nodo de inicio más cercano
        var start = ManagerNode.Instance.GetClosestNode(_shooter.transform.position, _shooter.zoneId);
        if (start == null)
        {
            return;
        }

        // Nodo destino aleatorio distinto al inicio
        NodePathfinding dest = start;
        int tries = 0;
        while (dest == start && tries < 10)
        {
            dest = zoneNodes[Random.Range(0, zoneNodes.Count)];
            tries++;
        }

        // Calcular A y asignar ruta
        _shooter.path = ManagerNode.Instance.FindPath(start, dest);
        _shooter.pathIndex = 0;
    }

    public void getrandomNum()
    {
        count++;

        if(count >= 3)
        {
            gotoplayer = true;
            _shooter.voyaPlayer = true;
            return;
        }
        GameManager.instance.StartCoroutine(GameManager.instance.randomeNum(0.5f, _shooter.maxPos, _shooter.minPos, _shooter.chance));
    }
}
