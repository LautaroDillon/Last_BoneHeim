using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomParticles : MonoBehaviour
{
    public ParticleSystem[] particles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetParticlesActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetParticlesActive(false);
        }
    }

    private void SetParticlesActive(bool active)
    {
        foreach (ParticleSystem ps in particles)
        {
            if (ps == null) continue;

            if (active && !ps.isPlaying)
                ps.Play();
            else if (!active && ps.isPlaying)
                ps.Stop();
        }
    }
}
