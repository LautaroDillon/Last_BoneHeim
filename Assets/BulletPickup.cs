using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.name);
        
        if (!other.CompareTag("Player"))
            return;

        PlayerWeapon weapon = other.GetComponentInParent<PlayerWeapon>();
        if (weapon != null)
        {
            AudioManager.instance?.PlaySFXClean("BulletRecovery", 1f);
            weapon.currentAmmo += 1;
        }
        
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
