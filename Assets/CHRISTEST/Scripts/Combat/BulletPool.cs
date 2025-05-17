using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("Pool Settings")]
    public GameObject bulletPrefab;
    public int poolSize = 50;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        FillPool();
    }

    void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }

        GameObject pooledBullet = bulletPool.Dequeue();
        pooledBullet.SetActive(true);
        return pooledBullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
