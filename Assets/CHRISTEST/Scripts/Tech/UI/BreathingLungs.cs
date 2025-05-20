using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingLungs : MonoBehaviour
{
    public float breathSpeed = 0.5f;     // Breaths per second
    public float scaleAmount = 0.05f;    // Scale intensity
    public bool uniformScale = false;    // If true, scales all axes

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = 1f + Mathf.Sin(Time.time * Mathf.PI * 2f * breathSpeed) * scaleAmount;

        if (uniformScale)
        {
            transform.localScale = originalScale * scaleFactor;
        }
        else
        {
            transform.localScale = new Vector3(
                originalScale.x,
                originalScale.y * scaleFactor,
                originalScale.z
            );
        }
    }
}
