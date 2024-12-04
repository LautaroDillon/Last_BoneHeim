using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ejarron : EnemisBehaivor, Idamagable
{
    public float dmg; // Daño
    public float life; // Vida inicial
    public float attackCooldown; // Tiempo entre ataques
    public float attackRange; // Rango de ataque
    public float followSpeed; // Velocidad al perseguir

    private float lastAttackTime;
    private bool isAwake = false; // Controla si el mímico está activo
    private NavMeshAgent agent;

    [SerializeField] GameObject intactObject; 
    [SerializeField] GameObject brokenObject;

    [Header("Sounds")]
    [SerializeField] private AudioClip jarronLaughClip;
    [SerializeField] private AudioClip jarronDamageClip;

    private void Awake()
    {
        currentlife = FlyweightPointer.Eshoot.maxLife + life;
        agent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        if (isAwake && currentlife > 0)
        {
            AttackOrFollowPlayer(); 
        }
    }

    private void AttackOrFollowPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > attackRange)
        {
            
            agent.SetDestination(player.position);
        }
        else
        {
            
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                player.GetComponent<Idamagable>().TakeDamage(dmg);
                lastAttackTime = Time.time;
                SoundManager.instance.PlaySound(jarronLaughClip, transform, 1f, false);
                Debug.Log("El mímico ataca al jugador.");
            }
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (isAwake) 
        {
            healParticle.SetActive(false);

            currentlife -= dmg;
            GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
            GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);

            Destroy(debris, 5);
            Destroy(acid, 15);

            if (currentlife <= 0)
            {
                Debug.Log("the skeleton received damage ");
                GameManager.instance.enemys.Remove(this.gameObject);

                if (gameObject.tag == "Skeleton")
                    SoundManager.instance.PlaySound(jarronDamageClip, transform, 1f, false);

                if (PlayerHealth.instance.isInReviveState)
                {
                    PlayerHealth.instance.enemyKilled = true;
                }

                Destroy(acid);
                Break();
                Destroy(this.gameObject, 1f);
                PlayerHealth.instance.life += 10;
                //Guns.instance.bulletsLeft += Random.Range(4, 5) + gun.killReward;
            }
        }
        else
        {
            isAwake = true;
        }
    }


    private void Break()
    {
        intactObject.SetActive(false);
        brokenObject.SetActive(true);
    }
}