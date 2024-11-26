using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;  // Bullet prefab to be instantiated
    public float shootInterval = 2f; // Time in seconds between each shot
    public float bulletSpeed = 10f;  // Speed of the bullet
    public Transform firePoint;      // Where the bullets should come from (typically a child object)

    [Header("Sounds")]
    [SerializeField] protected AudioClip turretShotClip;

    private void Start()
    {
        // Start shooting at regular intervals
        StartCoroutine(ShootAtInterval());
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private IEnumerator ShootAtInterval()
    {
        while (true)
        {
            SoundManager.instance.PlaySound(turretShotClip, transform, 1f, false);
            ShootBullet();

            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.velocity = firePoint.forward * bulletSpeed;

        Destroy(bullet, 5f);
    }
}
