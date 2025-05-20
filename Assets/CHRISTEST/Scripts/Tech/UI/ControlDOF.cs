using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ControlDOF : MonoBehaviour
{
    public Volume globalVolume;
    private DepthOfField dof;

    public float pauseFocalLength = 135f;
    public float normalFocalLength = 0f;

    private void Awake()
    {
        if (globalVolume.profile.TryGet(out dof))
        {
            dof.active = false;
        }
    }

    public void EnablePauseDOF()
    {
        if (dof != null)
        {
            dof.active = true;
            dof.focalLength.value = pauseFocalLength;
        }
    }

    public void DisablePauseDOF()
    {
        if (dof != null)
        {
            dof.focalLength.value = normalFocalLength;
            dof.active = false;
        }
    }

    public void LerpFocalLength(float from, float to, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FocalLengthTransition(from, to, duration));
    }

    private IEnumerator FocalLengthTransition(float from, float to, float duration)
    {
        float elapsed = 0f;

        if (dof != null)
            dof.active = true;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            dof.focalLength.value = Mathf.Lerp(from, to, t);
            yield return null;
        }

        dof.focalLength.value = to;
    }
}
