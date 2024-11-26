using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            StartCoroutine(timefall());
        }
    }

    private IEnumerator timefall()
    {
        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
}
