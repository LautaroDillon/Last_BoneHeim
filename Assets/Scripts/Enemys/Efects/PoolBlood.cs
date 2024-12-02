using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBlood : MonoBehaviour
{
    public int damagePerSecond;
    public float damageInterval;

    private Coroutine damageCoroutine;
    private PlayerHealth currentPlayer;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 11)
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                currentPlayer = playerHealth; // Guardamos la referencia al jugador
                if (damageCoroutine == null) // Verificamos que no haya un Coroutine activo
                {
                    damageCoroutine = StartCoroutine(DamagePlayerOverTime());
                }
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == 11 && other.gameObject.GetComponent<PlayerHealth>() == currentPlayer)
        {
            // Detenemos el Coroutine si el jugador actual sale de la trampa
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            currentPlayer = null;
        }
    }

    private IEnumerator DamagePlayerOverTime()
    {
        while (currentPlayer != null) // Mientras el jugador esté en la trampa
        {
            currentPlayer.TakeDamage(damagePerSecond);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
