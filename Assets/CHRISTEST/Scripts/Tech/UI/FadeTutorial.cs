using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeTutorial : MonoBehaviour
{
    public TMP_Text textToFade;
    public float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        SetTextAlpha(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeText(1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeText(0f));
        }
    }

    IEnumerator FadeText(float targetAlpha)
    {
        float startAlpha = textToFade.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            SetTextAlpha(alpha);
            yield return null;
        }

        SetTextAlpha(targetAlpha);
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = textToFade.color;
        color.a = alpha;
        textToFade.color = color;
    }
}
