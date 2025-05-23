using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform camHolder;
    public SensitivityController sensitivityController;

    private float xRotation;
    private float yRotation;

    private Tween fovTween;
    private Tween tiltTween;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityController.sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityController.sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        if (fovTween != null && fovTween.IsActive()) fovTween.Kill();
        fovTween = GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        if (tiltTween != null && tiltTween.IsActive()) tiltTween.Kill();
        tiltTween = transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
