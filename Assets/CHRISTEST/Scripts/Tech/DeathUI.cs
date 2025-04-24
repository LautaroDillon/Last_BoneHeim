using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathUI : MonoBehaviour
{
    public static bool deathUiActive;

    public GameObject deathUI;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private void Awake()
    {
        deathUI.SetActive(false);
        deathUiActive = false;
    }

    public void TriggerDeathFade()
    {
        StartCoroutine(FadeInCanvas());
    }
    public void TriggerDeathFadeOut()
    {
        StartCoroutine(FadeOutCanvas());
    }

    private IEnumerator FadeInCanvas()
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("Fade Canvas Group not assigned!");
            yield break;
        }

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        deathUiActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private IEnumerator FadeOutCanvas()
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogError("Fade Canvas Group not assigned!");
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        deathUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
