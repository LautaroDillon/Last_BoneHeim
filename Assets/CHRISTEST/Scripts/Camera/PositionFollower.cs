using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
    public Transform targetTransform;
    public Vector3 offset;
    public float followSpeed = 5f;

    private void LateUpdate()
    {
        Vector3 desiredPosition = targetTransform.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
    }
}
