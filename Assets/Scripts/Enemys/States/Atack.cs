using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Atack : IState
{

    NavMeshAgent _agent;
    E_Shooter _shooter;
    StateMachine _FSM;

    private float _attackDuration = 1.5f; // tiempo quieto mientras ataca
    private float _attackTimer;

    private float _cooldownTimer;

    public Atack(NavMeshAgent agent, E_Shooter shooter, StateMachine fSM)
    {
        _agent = agent;
        _shooter = shooter;
        _FSM = fSM;
    }

    public void OnEnter()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _attackTimer = 0f;
        _shooter.anim.SetFloat("Horizontal",0);
        _shooter.anim.SetFloat("Vertical",0);
        _cooldownTimer = 0f;
        _shooter.alreadyAttacked = false;
        _shooter.anim.SetBool("Walk", false);
    }

    public void Tick()
    {
        _attackTimer += Time.deltaTime;

        FaceTarget();

        float distance = Vector3.Distance(_shooter.transform.position, _shooter.player.position);
        if (distance > _shooter.attackRange)
        {
            _shooter.playerInAttackRange = false;
            return;
        }

        _agent.SetDestination(_shooter.transform.position);

        if (_attackTimer >= _attackDuration && !_shooter.alreadyAttacked)
        {
            _shooter.anim.SetTrigger("atack");
            _shooter.alreadyAttacked = true;
            AudioManager.instance.PlaySFXOneShot("ShooterAttack", 1f);
            Shoot();
            _cooldownTimer = _shooter.shotCooldown;
        }

        // Reducir cooldown
        if (_shooter.alreadyAttacked && _cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public void OnExit()
    {
        _agent.isStopped = false;
        //_shooter.alreadyAttacked = false;
    }

    private void FaceTarget()
    {
        var dir = (_shooter.player.position - _shooter.transform.position).normalized;
        dir.y = 0;
        _shooter.transform.rotation = Quaternion.Slerp(
            _shooter.transform.rotation,
            Quaternion.LookRotation(dir),
            10 * Time.deltaTime
        );
    }

    private void Shoot()
    {
        //_shooter.anim.SetTrigger("atack");

        var bullet = BuletManager.instance.GetBullet();
        bullet.transform.position = _shooter.firePoint.transform.position;
        bullet.transform.forward = (_shooter.player.position - _shooter.firePoint.transform.position).normalized;
        //bullet.GetComponent<Rigidbody>()?.velocity = bullet.transform.forward * _shooter.projectileSpeed;
    }
}
