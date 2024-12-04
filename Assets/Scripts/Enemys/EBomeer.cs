using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBomeer : EnemisBehaivor
{
    [Header("EBomeer")]
    [SerializeField] private float radius;
    [SerializeField] private int damage;
    public GameObject explosion;

    private void Awake()
    {
        currentlife = FlyweightPointer.Eboomer.maxLife;
        speed = FlyweightPointer.Eboomer.speed;
    }

    private void Update()
    {
        if (currentlife >= 0)
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
                    anim.SetBool("idle", true);
                    anim.SetBool("Moving", false);
                    break;
                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;
                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    anim.SetBool("idle", false);
                    anim.SetBool("Moving", true);
                    break;
            }
        }
        else
        {
                var lookpos = player.transform.position - transform.position;
                lookpos.y = 0;
                var rotation = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                 anim.SetBool("walk", true);
                 anim.SetBool("idle", false);

                // anim.SetBool("run", true);
                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(1);
            StartCoroutine("Explosion", 1.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            TakeDamage(1);
            StartCoroutine("Explosion", 1.5f);
        }
    }

    public IEnumerator Explosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.instance.enemys.Remove(this.gameObject);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.1f);
    }

    public override void TakeDamage(float dmg)
    {
        currentlife -= dmg;
        GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);

        Destroy(acid, 3);

        if (currentlife <= 0)
        {
            Ally.intance._enemies.Remove(this.gameObject);
            Destroy(acid);
            Destroy(this.gameObject, 0.1f);
        }
    }
}
