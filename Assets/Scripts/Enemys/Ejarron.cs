using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ejarron : EnemisBehaivor, Idamagable
{
    [Header("Ejarron")]
    [SerializeField] private float radius;
    [SerializeField] private int damage;
    [SerializeField] bool ishiding = true;

    [SerializeField] GameObject intactObject;
    [SerializeField] GameObject brokenObject;
    BoxCollider bc;

    private void Awake()
    {
        currentlife = FlyweightPointer.Eboomer.maxLife;
        speed = FlyweightPointer.Eboomer.speed;
    }

    private void Update()
    {
        if (currentlife >= 0 && !ishiding)
        {
            EnemiMovement();
        }
    }

    public void EnemiMovement()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > 15)
        {
            // anim.SetBool("run", false);

            cronometro += 1 * Time.deltaTime;
            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }
            switch (rutina)
            {
                case 0:
                    break;
                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;
                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    break;
            }
        }
        else
        {
            var lookpos = player.transform.position - transform.position;
            lookpos.y = 0;
            var rotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
            // anim.SetBool("walk", false);

            // anim.SetBool("run", true);
            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bullet" && ishiding)
        {

        }
    }

    public override void TakeDamage(float dmg)
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
                SoundManager.instance.PlaySound(skeletonDeathClip, transform, 1f, false);

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


    private void Break()
    {
        intactObject.SetActive(false);
        brokenObject.SetActive(true);
        bc.enabled = false;
    }
}