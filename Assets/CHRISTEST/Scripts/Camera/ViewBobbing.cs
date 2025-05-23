using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PositionFollower))]
public class ViewBobbing : MonoBehaviour
{
    public float EffectIntensity;
    public float EffectIntensityX;
    public float EffectSpeed;

    private PositionFollower FollowerInstance;
    public GameObject player;
    private PlayerMovement pm;
    private Vector3 OriginalOffset;
    private float SinTime;

    void Start()
    {
        FollowerInstance = GetComponent<PositionFollower>();
        pm = player.GetComponent<PlayerMovement>();
        OriginalOffset = FollowerInstance.offset;
    }

    void Update()
    {
        if(pm.sliding == false)
            ViewBob();

    }

    private void ViewBob()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));

        if (inputVector.magnitude > 0.5f)
            SinTime += Time.deltaTime * EffectSpeed;
        else
            SinTime = 0f;

        float sinAmountY = -Mathf.Abs(EffectIntensity * Mathf.Sin(SinTime));
        Vector3 sinAmount = FollowerInstance.transform.right * EffectIntensity * Mathf.Cos(SinTime) * EffectIntensityX;

        FollowerInstance.offset = new Vector3
        {
            x = OriginalOffset.x,
            y = OriginalOffset.y + sinAmountY,
            z = OriginalOffset.z
        };
    }
}