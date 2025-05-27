using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletprefab;
    public Transform[] bulletSpawnPoint;
    public float bulletSpeed = 30f;
    public float lifetime = 3f;

    [Header("Input Settings")]
    public KeyCode shootButton;
    public KeyCode reloadButton;

    int whatspawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(shootButton))
        {
            firewapon();
        } 
    }

    private void firewapon()
    {

        /*var bullet = BuletManager.instance.GetBullet();
            bullet.transform.position = bulletSpawnPoint[whatspawn].position;*/

        GameObject bullet = Instantiate(bulletprefab, bulletSpawnPoint[whatspawn].position, bulletSpawnPoint[whatspawn].rotation);

        bullet.GetComponent<Rigidbody>().AddForce(bulletSpawnPoint[whatspawn].forward.normalized * bulletSpeed, ForceMode.Impulse);

        Destroy(bullet, lifetime);
    }
}
