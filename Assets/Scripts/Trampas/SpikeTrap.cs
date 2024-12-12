using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] GameObject spikes;
    [SerializeField] Animation _spikeAnim;
    [SerializeField] private bool _spikesActive;
    private void Start()
    {
        //spikes.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Spikes Triggered!");
        if(other.gameObject.tag == "Player" && _spikesActive == false)
        {
            Invoke("SpikeActivated", 1.5f);
            Debug.Log("Spikes Activated!");
        }
        else if(_spikesActive == true)
        {
            Invoke("SpikeDeactivated", 3f);
            Debug.Log("Spikes Deactivated!");
        }
        
    }

    void SpikeActivated()
    {
        //spikes.gameObject.SetActive(true);
        if(_spikesActive == false)
        _spikeAnim.Play("SpikeTrigger");
        _spikesActive = true;


    }

    void SpikeDeactivated()
    {
        //spikes.gameObject.SetActive(false);
        if(_spikesActive == true)
        _spikeAnim.Play("SpikeRetract");
        _spikesActive = false;
    }
}
