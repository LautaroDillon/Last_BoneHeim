using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;
    public float rotateSpeed = 90f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + new Vector3(0, newY, 0);
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
