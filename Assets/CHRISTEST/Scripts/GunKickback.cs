using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunKickback : MonoBehaviour
{
    public Vector3 kickbackAmount = new Vector3(-0.1f, 0.0f, 0.0f);
    public float returnSpeed = 10f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Vector3 currentOffset;
    private bool isKickedBack = false;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (isKickedBack)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * returnSpeed);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * returnSpeed);

            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.001f)
                isKickedBack = false;
        }
    }

    public void ApplyKickback()
    {
        transform.localPosition += kickbackAmount;
        isKickedBack = true;
    }
}
