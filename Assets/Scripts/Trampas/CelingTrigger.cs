using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelingTrigger : MonoBehaviour
{
    public CeilingTrap trap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            trap.isActive = true;
    }
}
