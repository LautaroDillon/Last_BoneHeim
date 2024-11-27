using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinwheel : MonoBehaviour
{
    public float rotationSpeed = 100f;

    private void Update()
    {
        transform.Rotate(Vector3.down * rotationSpeed * Time.deltaTime);

    }
}
