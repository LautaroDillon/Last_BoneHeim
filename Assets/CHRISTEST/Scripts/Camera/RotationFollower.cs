using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFollower : MonoBehaviour
{
    public Transform Target;
    public float rotationSpeed = 5f;

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, Time.deltaTime * rotationSpeed);
    }
}
