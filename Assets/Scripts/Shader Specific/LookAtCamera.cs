using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        var dir = _cam.transform.position - transform.position;
        transform.forward = dir;
    }
}
