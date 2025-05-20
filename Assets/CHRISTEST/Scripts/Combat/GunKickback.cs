using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunKickback : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmount = 0.05f;
    public float swaySmoothness = 4f;

    [Header("Kickback Settings")]
    public Vector3 kickbackAmount = new Vector3(0f, 0f, -0.2f);
    public float kickbackRecoverySpeed = 8f;

    private Vector3 initialLocalPos;
    private Vector3 currentKickbackOffset;
    private Vector2 mouseInput;

    void Start()
    {
        initialLocalPos = transform.localPosition;
    }

    void Update()
    {
        HandleSway();
        RecoverKickback();
    }

    void HandleSway()
    {
        mouseInput.x = Input.GetAxis("Mouse X");
        mouseInput.y = Input.GetAxis("Mouse Y");

        Vector3 swayOffset = new Vector3(-mouseInput.x, -mouseInput.y, 0f) * swayAmount;
        Vector3 targetPos = initialLocalPos + swayOffset + currentKickbackOffset;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * swaySmoothness);
    }

    void RecoverKickback()
    {
        currentKickbackOffset = Vector3.Lerp(currentKickbackOffset, Vector3.zero, Time.deltaTime * kickbackRecoverySpeed);
    }

    public void ApplyKickback()
    {
        currentKickbackOffset += kickbackAmount;
    }
}
