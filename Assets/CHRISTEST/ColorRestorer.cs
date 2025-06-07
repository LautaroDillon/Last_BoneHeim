using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorRestorer : MonoBehaviour
{
    public Volume postProcessVolume;
    public float fadeDuration = 2f;

    private ColorAdjustments colorAdjustments;
    private bool restoringColor = false;
    private float timer = 0f;

    void Start()
    {
        if (postProcessVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.saturation.value = -100f;
        }
    }

    public void RestoreColor()
    {
        if (colorAdjustments != null)
        {
            restoringColor = true;
            timer = 0f;
        }
    }

    void Update()
    {
        if (restoringColor && colorAdjustments != null)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            colorAdjustments.saturation.value = Mathf.Lerp(-100f, 0f, t);

            if (t >= 1f)
                restoringColor = false;
        }
    }
}
