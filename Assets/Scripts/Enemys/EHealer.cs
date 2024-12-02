using UnityEngine;

public class EHealer : EnemisBehaivor, Idamagable
{
    [Header("Healer")]
    public float attackRange = 1f;
    public GameObject healerBuff;
    public bool canspawn = true;

    void Awake()
    {
        currentlife = FlyweightPointer.Ehealer.maxLife;
        speed = FlyweightPointer.Ehealer.speed;
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
        if (Vector3.Distance(transform.position, player.transform.position) > 20)
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
            var lookpos = transform.position - player.transform.position;
            lookpos.y = 0;
            var rotation = Quaternion.LookRotation(lookpos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2);
            anim.SetBool("Moving", true);
            anim.SetBool("idle", false);

            transform.Translate(Vector3.forward * 2 * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(1);
            Heal();
        }
    }


    void Heal()
    {
        if (currentlife <= 50 && canspawn)
        {
            GameObject healInstance = Instantiate(healerBuff, transform.position, Quaternion.identity);
            healInstance.GetComponent<Healing>().SetHealer(this);
            canspawn = false;
        }
    }
}

