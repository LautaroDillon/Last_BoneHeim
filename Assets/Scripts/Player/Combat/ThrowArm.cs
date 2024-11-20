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
        Destroy(gameObject, throwing.recoverArmMaxTime);
    }

    private void Update()
    {
        if(throwing.totalThrows == 0)
            throwing.recoverArmTime -= Time.deltaTime;
        if (throwing.recoverArmTime <= 0)
        {
            SpawnArm();
            throwing.recoverArmTime = throwing.recoverArmMaxTime;
            throwing.RestoreThrow();
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();
        if (other.gameObject.layer == 10)
        {
            Debug.Log("pego a enemigo");
            damagableInterface.TakeDamage(75);
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "DEBUG" || collision.gameObject.tag == "Lava")
        {
            Debug.Log("DEBUG: Returned Arm!");
            SpawnArm();
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Player")
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
            SoundManager.instance.PlaySound(armPickUpClip, transform, 1.5f, false);
            SoundManager.instance.PlaySound(armPickUp2Clip, transform, 1.5f, false);
            throwing.totalThrows += 1;
            print("Retrieved Arm!");
            Destroy(gameObject);
        }
    }

    void SpawnArm()
    {
        Instantiate(gameObject, target.transform.position, Quaternion.identity);
    }
}
