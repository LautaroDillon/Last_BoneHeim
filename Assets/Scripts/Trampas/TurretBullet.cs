using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] int turretDamage = 20;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();
        if(other.gameObject.tag == "Player" && damagableInterface != null)
        {
            Debug.Log("Hit Player!");
            damagableInterface.TakeDamage(turretDamage);
            Destroy(gameObject);
        }
    }
}
