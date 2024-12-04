using UnityEngine;

public class NormalSkeleton : EnemisBehaivor
{
    bool atacando;

    [Header("Attack Range")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected float shootRange;
    protected float DMG;
    protected float fireForce = 15f;
    [SerializeField] protected GameObject BulletPrefab;
    [SerializeField] protected float firerate, NextfireTime;
    public bool isTurret;

    public float projectileSpeed;

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

    public void EnemiMovement()
    {
        if (!canSeePlayer)
        {
            // anim.SetBool("run", false);
            if (!isTurret)
            {

                cronometro += 1 * Time.deltaTime;
                if (cronometro >= 4)
                {
                    rutina = Random.Range(0, 2);
                    cronometro = 0;
                }
                switch (rutina)
                {
                    case 0:
                        //anim.SetBool("Moving", false);
                        //anim.SetBool("IsShootiing", false);
                        //anim.SetBool("idle", true);

                        break;
                    case 1:
                        //anim.SetBool("Moving", false);
                        //anim.SetBool("IsShootiing", false);
                        grado = Random.Range(0, 360);
                        angulo = Quaternion.Euler(0, grado, 0);
                        rutina++;
                        break;
                    case 2:
                        //anim.SetBool("IsShootiing", false);
                        //anim.SetBool("idle", false);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                        transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                        //anim.SetBool("Moving", true);
                        break;
                }
            }
        }
        else
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Debug.Log("veo player");
            if (distanceToPlayer > shootRange && !atacando && !isTurret)
            {
                //anim.SetBool("idle", false);
                //anim.SetBool("IsShootiing", false);

                var lookpos = player.transform.position - transform.position;
                lookpos.y = 0;
                var rotation = Quaternion.LookRotation(lookpos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
                //anim.SetBool("Moving", true);

                transform.Translate(Vector3.forward * 2 * Time.deltaTime);
            }
            else if (distanceToPlayer <= shootRange)
            {
                    if (canSeePlayer)
                    {
                        //anim.SetBool("Moving", false);
                        //anim.SetBool("idle", false);
                        Debug.Log("Disparando");

                        var lookPos = player.transform.position - transform.position;
                        lookPos.y = 0;
                        var rotation = Quaternion.LookRotation(lookPos);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);

                        if (Canfire())
                        {
                            Shoot();
                        }

                        //anim.SetBool("IsShootiing", true);

                        atacando = true;
                        finanim();
                    }
                }

        }
    }

    bool Canfire()
    {
        return Time.time >= NextfireTime;
    }

    public void Shoot()
    {
        Vector3 directionToPlayer = (player.transform.position - firePoint.position).normalized;

        var bullet = BuletManager.instance.GetBullet();
        bullet.transform.position = firePoint.transform.position;
        bullet.transform.forward = directionToPlayer;


        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Debug.Log("hola");
            rb.velocity = directionToPlayer * projectileSpeed;
        }

        NextfireTime = Time.time + 1f / firerate;
    }

    public void finanim()
    {
         //anim.SetBool("IsShootiing", false);
        atacando = false;
    }
}