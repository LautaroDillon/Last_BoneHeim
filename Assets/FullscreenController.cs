using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FullscreenController : MonoBehaviour
{
    [SerializeField] private float _displayTime = 1.5f;
    [SerializeField] private float fadeoutTime = 0.5f;

    [SerializeField] private ScriptableRendererFeature _fullscreenShader;
    [SerializeField] private Material _material;

    private int _voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");
    private int _vignetteIntensity = Shader.PropertyToID("_Intensity");

    private const float VORONOI_INTENSITY_START_AMOUNT = 1f;
    private const float VIGNETTE_INTENSITY_START_AMOUNT = 1.2f;

    private void Start()
    {
        _fullscreenShader.SetActive(false);
    }

    public IEnumerator HurtShader()
    {
        _fullscreenShader.SetActive(true);
        _material.SetFloat(_voronoiIntensity, VORONOI_INTENSITY_START_AMOUNT);
        _material.SetFloat(_vignetteIntensity, VIGNETTE_INTENSITY_START_AMOUNT);

        yield return new WaitForSeconds(_displayTime);

        float timeElapsed = 0f;
        while(timeElapsed < fadeoutTime)
        {
            timeElapsed += Time.deltaTime;
            float lerpedVoronoi = Mathf.Lerp(VORONOI_INTENSITY_START_AMOUNT, 0f, (timeElapsed / fadeoutTime));
            float lerpedVignette = Mathf.Lerp(VIGNETTE_INTENSITY_START_AMOUNT, 0f, (timeElapsed / fadeoutTime));

            _material.SetFloat(_voronoiIntensity, lerpedVoronoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);

            yield return null;
        }
        _fullscreenShader.SetActive(false);
    }
}
