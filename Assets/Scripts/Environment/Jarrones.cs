using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jarrones : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
        }
    }
}
