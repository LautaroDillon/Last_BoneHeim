using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuletManager : MonoBehaviour
{
    public static BuletManager instance;

    [Header("Factory_Objetcpool")]
    [SerializeField] public Bullet prefab;
    ObjectPool<Bullet> _objectPool;
    Factory<Bullet> _factory;

    private void Awake()
    {
        instance = this;
        _factory = new BulletFactory(prefab);
        _objectPool = new ObjectPool<Bullet>(_factory.GetObj, Bullet.TurnOff, Bullet.TurnOn, 15);
    }

    public Bullet GetBullet()
    {
        var x = _objectPool.Get();
        x.AddReference(_objectPool);
        return x;
    }

    public void realese(Bullet B)
    {
        _objectPool.StockAdd(B);
    }

}
