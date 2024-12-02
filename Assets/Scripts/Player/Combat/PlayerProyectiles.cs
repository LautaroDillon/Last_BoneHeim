using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProyectiles : MonoBehaviour
{
    public float lifeTime = 2f;
    private void Awake()
    {
        Destroy(this.gameObject, lifeTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();

        if (other.gameObject.layer == 10 && damagableInterface != null)
        {
            Debug.Log("pego a enemigo");
            damagableInterface.TakeDamage(FlyweightPointer.Player.Damage);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);

        }
    }

}
