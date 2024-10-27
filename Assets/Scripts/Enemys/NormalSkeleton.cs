using UnityEngine;

public class NormalSkeleton : EnemisBehaivor
{
    public static NormalSkeleton instance;

    bool atacando;

    [Header("atack range")]
    [SerializeField] protected Transform firePoint;
    protected float DMG;
    protected float fireForce = 15f;
    [SerializeField] protected GameObject BulletPrefab;
    [SerializeField] protected float firerate, NextfireTime;



    private void Awake()
    {
        currentlife = FlyweightPointer.Eshoot.maxLife;
        instance = this;
    }

    private void Update()
    {
        if (currentlife >= 0)
        {
            EnemiMovement();
        }
    }

    public  void EnemiMovement()
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
            Debug.Log("veo player");
            if (Vector3.Distance(transform.position, player.transform.position) > 10 && !atacando)
            {
                var lookpos = player.transform.position - transform.position;
                lookpos.y = 0;
                var rotation = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                // anim.SetBool("walk", false);

                // anim.SetBool("run", true);
                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
            }
            else
            {
                Debug.Log("ataco");
                // anim.SetBool("run", true);
                // anim.SetBool("walk", false);
                var lookpos = player.transform.position - transform.position;
                lookpos.y = 0;
                var rotation = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                if (Canfire())
                {
                    Shoot();
                }


                // anim.SetBool("atack", true);
                atacando = true;
                finanim();
            }

        }
    }

    bool Canfire()
    {
        return Time.time >= NextfireTime;
    }

    public void Shoot()
    {
        var bullet = BuletManager.instance.GetBullet();
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.forward = firePoint.transform.forward;

        NextfireTime = Time.time + 1f / firerate;
    }

    public void finanim()
    {
        //anim.SetBool("atack", false);
        atacando = false;
    }
}