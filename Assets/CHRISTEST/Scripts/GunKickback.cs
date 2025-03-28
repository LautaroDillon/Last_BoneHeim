using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunKickback : MonoBehaviour
{
    public Vector3 kickbackAmount = new Vector3(-0.1f, 0.0f, 0.0f);
    public float returnSpeed = 5f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float kickbackTimer;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (kickbackTimer > 0)
        {
            kickbackTimer -= Time.deltaTime;
            if (kickbackTimer <= 0)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, returnSpeed * Time.deltaTime);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, returnSpeed * Time.deltaTime);
            }
        }
    }

    public void ApplyKickback()
    {
        transform.localPosition += kickbackAmount;
        kickbackTimer = 0.1f;
    }
}
