using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoUI : MonoBehaviour
{
    public float angleRange = 15f;   // Maximum rotation angle (degrees)
    public float frequency = 1f;     // Speed of the sway

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * frequency) * angleRange;
        transform.localRotation = initialRotation * Quaternion.Euler(0f, angle, 0f);
    }
}
