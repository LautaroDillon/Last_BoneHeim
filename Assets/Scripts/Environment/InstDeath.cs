using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InstDeath : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        PlayerHealth damagableInterface = collision.gameObject.GetComponent<PlayerHealth>();

        if (collision.gameObject.layer == 11 && damagableInterface != null)
        {
            damagableInterface.TakeDamage(50);
        }
    }
}
