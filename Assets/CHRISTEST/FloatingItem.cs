using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float floatAmount = 0.5f;
    public bool floating = true;

    private Vector3 localOffset;
    private float offset;

    void Start()
    {
        localOffset = transform.localPosition;
        offset = Random.Range(0f, 1f * Mathf.PI);
    }

    void Update()
    {
        if (!floating) return;

        Vector3 basePos = transform.parent ? transform.parent.position : transform.position;

        float newY = basePos.y + Mathf.Sin(Time.time * floatSpeed + offset) * floatAmount;

        transform.position = new Vector3(basePos.x, newY, basePos.z);
    }
}
