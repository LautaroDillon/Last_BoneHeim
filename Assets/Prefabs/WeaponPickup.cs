using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Guns.instance.ResetGun();
            Guns.instance.KnuckleBuster();
            Destroy(gameObject);
        }
    }
}
