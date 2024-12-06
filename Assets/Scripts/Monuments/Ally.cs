using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour, Idamagable
{
    public static Ally intance;

    public Transform _player;
    public float _dmg;
    public float _currentLife;
    public float _attackRange;
    public float _followSpeed;
    public float _stopDistance;
    public List<GameObject> _enemies;
    public float timeLife;
    public float attackCooldown = 1.0f;

    private GameObject _targetEnemy;
    private float lastAttackTime;

    [SerializeField] public Animator anim;


    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;

        if (_currentLife <= 0)
        {
            Debug.Log("El aliado ha muerto.");
            Destroy(this.gameObject);
        }
    }

    private void Awake()
    {
        intance = this;
        _enemies = GameManager.instance.enemys;
        _player = GameManager.instance.thisIsPlayer;
        Destroy(this.gameObject, timeLife);
    }

    private void Update()
    {
        _enemies = GameManager.instance.enemys;

        if (_currentLife > 0)
        {
            FindClosestEnemy();

            if (_targetEnemy != null)
            {
                AttackEnemy();
            }
            else
            {
                FollowPlayer();
            }
        }
    }

    public void resetAnims()
    {
        anim.SetBool("Idle", false);
        anim.SetBool("Punch", false);
        anim.SetBool("Walk", false);
    }

    public void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceToPlayer > _stopDistance)
        {
            resetAnims();
            anim.SetBool("Walk", true);
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _followSpeed * Time.deltaTime);
        }
        else
        {
            resetAnims();
            anim.SetBool("Idle", true);

        }
    }

    public void FindClosestEnemy()
    {
        _targetEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in _enemies)
        {
            if(enemy == null)
            {
                _enemies.Remove(enemy);
                return;
            }

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance && distanceToEnemy <= _attackRange)
            {
                closestDistance = distanceToEnemy;
                _targetEnemy = enemy;
            }
        }
    }

    public void AttackEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position, _followSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetEnemy.transform.position) <= _attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                resetAnims();
                anim.SetBool("Punch", true);
                Debug.Log("Atacando al enemigo " + _targetEnemy.name);
                _targetEnemy.GetComponent<Idamagable>().TakeDamage(_dmg);
                lastAttackTime = Time.time;
            }
        }
    }
}
