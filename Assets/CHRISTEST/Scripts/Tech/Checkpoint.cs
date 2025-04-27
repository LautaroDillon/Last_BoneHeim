using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated)
            return;

        if (other.gameObject.tag == "Player")
        {
            isActivated = true;
            Debug.Log("Checkpoint set!");
            AudioManager.instance.PlaySFXOneShot("Checkpoint", 1f);
            CheckpointManager.instance.SetCheckpoint(transform.position + Vector3.up * 1f);
        }
    }
}
