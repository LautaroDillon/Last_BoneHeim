using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public GameObject breakEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arm"))
        {
            BreakWall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Arm"))
        {
            BreakWall();
        }
    }

    private void BreakWall()
    {
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
