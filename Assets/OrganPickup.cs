using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OrganPickup : MonoBehaviour
{
    [Header("References")]
    public FullscreenController fullscreenController;

    [Header("Damage Shader")]
    [SerializeField] private ScriptableRendererFeature _fullScreenDamage;
    [SerializeField] private Material _material;

    [Header("Shader Values")]
    [SerializeField] private float _damageDisplayTime = 1.5f;
    [SerializeField] private float _damageFadeoutTime = 0.5f;
    private int _voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");
    private int _vignetteIntensity = Shader.PropertyToID("_Intensity");
    private const float VORONOI_INTENSITY_START_AMOUNT = 2f;
    private const float VIGNETTE_INTENSITY_START_AMOUNT = 2.2f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Organ")
        {
            StartCoroutine(OrganShader());
        }
    }

    public IEnumerator OrganShader()
    {
        _fullScreenDamage.SetActive(true);
        _material.SetFloat(_voronoiIntensity, VORONOI_INTENSITY_START_AMOUNT);
        _material.SetFloat(_vignetteIntensity, VIGNETTE_INTENSITY_START_AMOUNT);

        yield return new WaitForSeconds(_damageDisplayTime);

        float timeElapsed = 0f;
        while (timeElapsed < _damageFadeoutTime)
        {
            timeElapsed += Time.deltaTime;
            float lerpedVoronoi = Mathf.Lerp(VORONOI_INTENSITY_START_AMOUNT, 0f, (timeElapsed / _damageFadeoutTime));
            float lerpedVignette = Mathf.Lerp(VIGNETTE_INTENSITY_START_AMOUNT, 0f, (timeElapsed / _damageFadeoutTime));

            _material.SetFloat(_voronoiIntensity, lerpedVoronoi);
            _material.SetFloat(_vignetteIntensity, lerpedVignette);

            yield return null;
        }
        _fullScreenDamage.SetActive(false);
    }
}
