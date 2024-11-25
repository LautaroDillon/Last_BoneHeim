using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] protected AudioClip bossTriggerClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            SoundManager.instance.PlaySound(bossTriggerClip, transform, 0.5f, false);
    }
}
