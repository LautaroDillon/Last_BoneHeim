using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] int spikeDamage = 50;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();
        Debug.Log("Spikes activated!");
        if (other.gameObject.tag == "Player" && damagableInterface != null)
        {
            damagableInterface.TakeDamage(spikeDamage);
        }
    }
}
