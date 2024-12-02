using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBlood : MonoBehaviour
{
    public int damagePerSecond = 2;
    public float damageInterval = 1f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                StartCoroutine(DamagePlayerOverTime(playerHealth));
            }
        }
    }

    IEnumerator DamagePlayerOverTime(PlayerHealth playerHealth)
    {
        while (playerHealth != null)
        {
            playerHealth.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
