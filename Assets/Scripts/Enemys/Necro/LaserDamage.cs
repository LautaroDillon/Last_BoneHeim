using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
            PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();

            if (other.gameObject.layer == 11 && damagableInterface != null)
            {
                damagableInterface.TakeDamage(FlyweightPointer.Eshoot.Damage);
            }
    }
}
