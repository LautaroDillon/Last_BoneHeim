using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowArm : MonoBehaviour
{
    Guns guns;
    Throwing throwing;
    [SerializeField] private AudioClip armPickUpClip;
    [SerializeField] private AudioClip armPickUp2Clip;
    public GameObject target;

    void Awake()
    {
        throwing = GameObject.Find("Player").GetComponent<Throwing>();
        guns = GameObject.Find("Gun").GetComponent<Guns>();
    }

    private void Start()
    {
        target = GameObject.Find("Player");
    }

    public void OnTriggerEnter(Collider other)
    {
        Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();
        if (other.gameObject.layer == 10)
        {
            Debug.Log("pego a enemigo");
            damagableInterface.TakeDamage(100);
            if (guns.isSkeleton == true)
            {
                guns.bulletsLeft += 5;
            }
            if (guns.isInvoker == true)
            {
                guns.bulletsLeft += 3;
            }
            if (guns.isKnuckle == true)
            {
                guns.bulletsLeft += 20;
            }
            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.OnEnemyKilled();
            }
        }
        if(other.gameObject.tag == "DEBUG" || other.gameObject.tag == "Lava")
        {
            Debug.Log("DEBUG: Returned Arm!");
            Instantiate(gameObject, target.transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(guns.isSkeleton == true)
            {
                guns.magazineSize += 5;
            }
            if (guns.isInvoker == true)
            {
                guns.magazineSize += 3;
            }
            if (guns.isKnuckle == true)
            {
                guns.magazineSize += 20;
            }
            SoundManager.instance.PlaySound(armPickUpClip, transform, 1f);
            SoundManager.instance.PlaySound(armPickUp2Clip, transform, 1f);
            throwing.totalThrows += 1;
            print("Retrieved Arm!");
            Destroy(gameObject);
        }
    }
}
