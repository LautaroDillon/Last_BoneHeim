using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Closed : MonoBehaviour
{
    public Fall fall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            fall.Activate();
        }
    }
}
