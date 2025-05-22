using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage;
    public PlayerHealth damagableInterface;

    private void Start()
    {
        damagableInterface = GameManager.instance.player.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Lava Triggered");
        if (other.gameObject.layer == 3 && damagableInterface != null)
        {
            Debug.Log("Lava Damage");
            StartCoroutine(waitforsecond(1f));
        }
    }

    public IEnumerator waitforsecond(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("wait for second");
        damagableInterface.TakeDamage(damage);
        StartCoroutine(waitforsecond(1f));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            Debug.Log("Lava Exit");
            StopAllCoroutines();
        }
    }
}
