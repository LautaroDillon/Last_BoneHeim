using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalpicaduraManager : MonoBehaviour
{
    public ParticleSystem pisoFX;
    public ParticleSystem paredFX;

    void OnParticleCollision(GameObject other)
    {
        // Detecta si golpeó piso
        if (other.CompareTag("Groundpart"))
        {
            if (pisoFX != null)
            {
                pisoFX.transform.position = transform.position;
                pisoFX.Play();
            }
        }
        // Detecta si golpeó pared
        else if (other.CompareTag("WallPart"))
        {
            if (paredFX != null)
            {
                paredFX.transform.position = transform.position;
                paredFX.Play();
            }
        }
    }
}
