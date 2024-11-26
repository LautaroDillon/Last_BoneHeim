using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth damagableInterface = collision.gameObject.GetComponent<PlayerHealth>();

        if (collision.gameObject.layer == 11 && damagableInterface != null)
        {
            damagableInterface.TakeDamage(damage);
            
        }
        Destroy(gameObject);
    }
}
