using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    public int damagePerSecond; 
    public float damageInterval;
    public ParticleSystem _firemain;

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
                    _firemain.Play(true);
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
                _firemain.Stop(true);
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