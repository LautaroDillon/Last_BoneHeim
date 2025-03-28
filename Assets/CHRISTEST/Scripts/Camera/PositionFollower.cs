using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;

    void Update()
    {
        transform.position = targetTransform.position + offset;
    }
}
