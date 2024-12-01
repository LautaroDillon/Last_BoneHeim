using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowArm : MonoBehaviour
{
    Guns guns;
    Throwing throwing;

    void Awake()
    {
        throwing = GameObject.Find("Player").GetComponent<Throwing>();
        guns = GameObject.Find("Gun").GetComponent<Guns>();
    }

    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();
        if (other.gameObject.layer == 10)
        {
            Debug.Log("pego a enemigo");
            damagableInterface.TakeDamage(100);
            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.OnEnemyKilled();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(guns.isSkeleton == true)
            {
                guns.magazineSize += 5;
                guns.bulletsLeft += 5;
            }
            if (guns.isInvoker == true)
            {
                guns.magazineSize += 3;
                guns.bulletsLeft += 3;
            }
            if (guns.isKnuckle == true)
            {
                guns.magazineSize += 20;
                guns.bulletsLeft += 20;
            }
            throwing.totalThrows += 1;
            print("Retrieved Arm!");
            Destroy(gameObject);
        }
    }
}
