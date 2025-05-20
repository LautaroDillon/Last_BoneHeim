using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatingHeart : MonoBehaviour
{
    public float baseBeatSpeed = 1f;
    public float maxBeatSpeed = 2f;

    public float baseScaleAmount = 0.05f;
    public float maxScaleAmount = 0.2f;

    public AnimationCurve beatCurve;

    private Vector3 originalScale;
    private float time;

    // Reference to player's health component
    public PlayerHealth playerHealth;

    void Start()
    {
        originalScale = transform.localScale;

        if (beatCurve == null || beatCurve.length == 0)
        {
            beatCurve = new AnimationCurve(
                new Keyframe(0f, 0f),
                new Keyframe(0.2f, 1f),
                new Keyframe(0.4f, 0f),
                new Keyframe(0.6f, 0.6f),
                new Keyframe(1f, 0f)
            );
        }

        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth == null)
         return;

        float healthPercent = Mathf.Clamp01(playerHealth.currentHealth / playerHealth.maxHealth);

        float beatSpeed = Mathf.Lerp(maxBeatSpeed, baseBeatSpeed, healthPercent);
        float scaleAmount = Mathf.Lerp(maxScaleAmount, baseScaleAmount, healthPercent);

        time += Time.deltaTime * beatSpeed;
        float curveValue = beatCurve.Evaluate(time % 1f); 

        float scaleFactor = 1f + curveValue * scaleAmount;
        transform.localScale = originalScale * scaleFactor;
    }
}
