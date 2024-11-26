using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] GameObject spikes;
    private void Start()
    {
        spikes.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Spikes Triggered!");
        if(other.gameObject.tag == "Player")
        {
            Invoke("SpikeActivated", 1.5f);
            Debug.Log("Spikes Activated!");
        }
        Invoke("SpikeDeactivated", 3f);
        Debug.Log("Spikes Deactivated!");
    }

    void SpikeActivated()
    {
        spikes.gameObject.SetActive(true);
    }

    void SpikeDeactivated()
    {
        spikes.gameObject.SetActive(false);
    }
}
