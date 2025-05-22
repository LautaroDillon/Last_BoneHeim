using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitmarkerController : MonoBehaviour
{
    public CanvasGroup hitmarkerCanvasGroup;
    public float showDuration = 0.2f;
    public float fadeSpeed = 5f;

    private float timer = 0f;

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            hitmarkerCanvasGroup.alpha = 1f;
        }
        else
        {
            hitmarkerCanvasGroup.alpha = Mathf.Lerp(hitmarkerCanvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
        }
    }

    public void ShowHitmarker()
    {
        timer = showDuration;
        hitmarkerCanvasGroup.alpha = 1f;
    }
}
