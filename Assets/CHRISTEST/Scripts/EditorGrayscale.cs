using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class EditorSafeGrayscale : MonoBehaviour
{
    public Volume volume;

    void Update()
    {
        if (volume != null && volume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Disable grayscale in Edit mode
                colorAdjustments.active = false;
            }
            else
            {
                // Enable grayscale in Play mode
                colorAdjustments.active = true;
            }
#else
            // In build, always keep it active
            colorAdjustments.active = true;
#endif
        }
    }
}
