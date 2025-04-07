using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
    public GameObject respawnPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = respawnPlayer.transform.position;
            AudioManager.instance.PlaySFXOneShot("Respawn", 1f);
        }
    }
}
