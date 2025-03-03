using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Variables")]
    public float speed;
    public float counter;
    public float lifetime;
    ObjectPool<Bullet> _objectPool;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        counter += Time.deltaTime;

        if (counter >= lifetime)
        {
            _objectPool.StockAdd(this);
        }
    }

    public void AddReference(ObjectPool<Bullet> op)
    {
        _objectPool = op;
    }

    public static void TurnOff(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    public static void TurnOn(Bullet bullet)
    {
        bullet.counter = 0;
        bullet.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerHealth damagableInterface = collision.gameObject.GetComponent<PlayerHealth>();
        Debug.Log("coliciono con algo");

        if (collision.gameObject.layer == 11 && damagableInterface != null)
        {
            Debug.Log("player recibe daño"); 
            TurnOff(this);
            _objectPool.StockAdd(this);
            damagableInterface.TakeDamage(FlyweightPointer.Eshoot.Damage);
        }
        else if(collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
        {
            TurnOff(this);
            _objectPool.StockAdd(this);
        }
    }
}