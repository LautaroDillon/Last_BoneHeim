using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowArm : MonoBehaviour
{
    Guns guns;
    Throwing throwing;
    [SerializeField] private AudioClip armPickUpClip;
    [SerializeField] private AudioClip armPickUp2Clip;
    public GameObject target;

    private bool ammoCharged;

    void Awake()
    {
        throwing = GameObject.Find("Player").GetComponent<Throwing>();
        guns = GameObject.Find("Gun").GetComponent<Guns>();
    }

    private void Start()
    {
        ammoCharged = false;
        target = GameObject.Find("Player");
        Destroy(gameObject, throwing.recoverArmMaxTime);
    }

    private void Update()
    {
        if(throwing.totalThrows == 0)
        {
            throwing.recoverArmTime -= Time.deltaTime;
        }
        if (throwing.recoverArmTime <= 0)
        {
            AdjustMagazineSizeForWeapon();
            SpawnArm();
            throwing.RestoreThrow();
            throwing.armPrefab.gameObject.SetActive(true);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        IDamagable damagableInterface = other.gameObject.GetComponent<IDamagable>();
        if (other.gameObject.layer == 10)
        {
            Debug.Log("pego a enemigo");
            damagableInterface.TakeDamage(75);
            ModifyAmmoBasedOnWeapon();
            ammoCharged = true;
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
            HandleArmPickup();
            if (ammoCharged == true)
                ModifyAmmoBasedOnWeapon();
            throwing.totalThrows += 1;
            print("Retrieved Arm!");
            FullscreenShader.instance.armShaderEnabled = true;
            Destroy(gameObject);
        }
    }
    private void HandleArmPickup()
    {
        CameraShake.Shake(0.4f, 0.4f);

        AdjustMagazineSizeForWeapon();

        throwing.armPrefab.gameObject.SetActive(true);
        throwing.recoverArmTime = throwing.recoverArmMaxTime;

        SoundManager.instance.PlaySound(armPickUpClip, transform, 1.5f, false);
        SoundManager.instance.PlaySound(armPickUp2Clip, transform, 1.5f, false);

        throwing.totalThrows += 1;
        Destroy(gameObject);
    }

    private void ModifyAmmoBasedOnWeapon()
    {
        if (guns.isSkeleton)
            guns.bulletsLeft += 5;
        else if (guns.isInvoker)
            guns.bulletsLeft += 3;
        else if (guns.isKnuckle)
            guns.bulletsLeft += 20;
        else if (guns.isTeeth)
            guns.bulletsLeft += 16;
        else if (guns.isNail)
            guns.bulletsLeft += 2;
        else if (guns.isParasite)
            guns.bulletsLeft += 6;
    }

    private void AdjustMagazineSizeForWeapon()
    {
        if (guns.isSkeleton) guns.magazineSize = 10;
        else if (guns.isInvoker) guns.magazineSize = 6;
        else if (guns.isKnuckle) guns.magazineSize = 40;
        else if (guns.isTeeth) guns.magazineSize = 32;
        else if (guns.isNail) guns.magazineSize = 4;
        else if (guns.isParasite) guns.magazineSize = 12;
    }

    void SpawnArm()
    {
        Instantiate(gameObject, target.transform.position, Quaternion.identity);
    }
}
