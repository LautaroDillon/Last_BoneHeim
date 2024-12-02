using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_fire : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();
        
        if (other.gameObject.layer == 11)
        {
            if (other.gameObject.layer == 11 && damagableInterface != null)
            {
                damagableInterface.TakeDamage(FlyweightPointer.Eshoot.Damage);
            }
        }

        Destroy(gameObject);
    }
}
