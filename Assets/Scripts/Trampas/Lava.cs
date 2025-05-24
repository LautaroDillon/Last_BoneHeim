using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage = 10f;
    public float damageInterval = 1f;

    private PlayerHealth damagableInterface;
    private Coroutine damageCoroutine;

    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            damagableInterface = GameManager.instance.player.GetComponent<PlayerHealth>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 && damagableInterface != null)
        {
            Debug.Log("Player entered lava");
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyLavaDamage());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            Debug.Log("Player exited lava");
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator ApplyLavaDamage()
    {
        while (true)
        {
            damagableInterface.TakeDamage(damage);
            Debug.Log("Lava applied damage: " + damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
