using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }
    private void LateUpdate()
    {
        transform.forward = _camera.transform.forward;
    }
}
