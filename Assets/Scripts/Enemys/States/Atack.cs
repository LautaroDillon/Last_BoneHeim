using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Atack : IState
{

    NavMeshAgent _agent;
    E_Shooter _shooter;
    StateMachine _FSM;

    private float _cooldownTimer;

    public Atack(NavMeshAgent agent, E_Shooter shooter, StateMachine fSM)
    {
        _agent = agent;
        _shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        Debug.Log("Atack OnEnter");
        _agent.speed = 0f; // Detener al enemigo
    }

    public void Tick()
    {
        if(_shooter.player == null)  return;

        Vector3 dir = (_shooter.player.position - _shooter.transform.position).normalized;
        dir.y = 0;
        _shooter.transform.rotation = Quaternion.Slerp(_shooter.transform.rotation, Quaternion.LookRotation(dir), 5 * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(_shooter.transform.position, _shooter.player.position);

        if (distanceToPlayer > _shooter.attackRange)
        {
            _shooter.playerInAttackRange = false;
            return;
        }

        _agent.SetDestination(_shooter.transform.position); // Se queda quieto

        if (!_shooter.alreadyAttacked)
        {
            Shoot();
            _shooter.anim.SetTrigger("Attack");
            _shooter.alreadyAttacked = true;
            _cooldownTimer = _shooter.shotCooldown;
        }

    }

    public void OnExit()
    {
        Debug.Log("Atack OnExit");
    }

    private void Shoot()
    {
        Vector3 directionToPlayer = (_shooter.player.position - _shooter.firePoint.transform.position).normalized;

        var bullet = BuletManager.instance.GetBullet();
        bullet.transform.position = _shooter.firePoint.transform.position;
        bullet.transform.forward = directionToPlayer;
    }
}
