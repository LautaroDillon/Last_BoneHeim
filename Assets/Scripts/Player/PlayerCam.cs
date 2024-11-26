using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform camHolder;
    public Slider slider;

    [Header("Variables")]
    public float sensX;
    public float sensY;
    public float _tiltAmount = 5;
    public float _rotationSpeed = 0.5f;

    float yRotation;
    float xRotation;

    private void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f); // Default to 1 if not set
        slider.value = Mathf.Clamp(savedSensitivity, slider.minValue, slider.maxValue);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Sensitivity();
        MouseInput();
    }

    void MouseInput()
    {
        float mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        sensX = mouseSensitivity * Time.timeScale * 0.25f; // Scale it to a more reasonable value
        sensY = mouseSensitivity * Time.timeScale * 0.25f;

        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        //evita que podamos dar vueltas mirando constantemente para arriba
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }

    public void Tilt()
    {
        float rotZ = -Input.GetAxis("Horizontal") * _tiltAmount;

        Quaternion finalRot = Quaternion.Euler(xRotation, yRotation, rotZ);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, finalRot, _rotationSpeed);
    }

    public void Sensitivity()
    {
        float newSensitivity = slider.value;
        PlayerPrefs.SetFloat("MouseSensitivity", newSensitivity);
        PlayerPrefs.Save(); // Save preferences
        sensX = newSensitivity;
        sensY = newSensitivity;
    }
}