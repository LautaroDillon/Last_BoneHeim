using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();

        if (other.gameObject.layer == 11 && damagableInterface != null)
        {
            damagableInterface.TakeDamage(616);
        }
    }
}
