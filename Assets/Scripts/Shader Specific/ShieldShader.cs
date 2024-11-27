using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldShader : MonoBehaviour
{
    Renderer _renderer;
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] float _DisplacementMagnitude;
    [SerializeField] float _LerpSpeed;
    [SerializeField] float _DissolveSpeed;
    private bool _shieldOn;

    Coroutine _dissolveCoroutine;

    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitShield(Vector3 hitpos)
    {
        _renderer.material.SetVector("_HitPosition", hitpos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());


    }

    public void OpenCloseShield()
    {
        float target = 1;

        if (_shieldOn)
        {
            target = 0;
        }
        _shieldOn = !_shieldOn;
        if(_dissolveCoroutine != null)
        {
            StopCoroutine(_dissolveCoroutine);
        }
        _dissolveCoroutine = StartCoroutine(Coroutine_DissolveShield(target));
    }

    IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_DisplacementStrenght",_DisplacementCurve.Evaluate(lerp)*_DisplacementMagnitude);
            lerp += Time.deltaTime * _LerpSpeed;
            yield return null;
        }
    }

    IEnumerator Coroutine_DissolveShield(float target)
    {
        float start = _renderer.material.GetFloat("_Dissolve");
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_Dissolve", Mathf.Lerp(start,target,lerp));
            lerp += Time.deltaTime * _DissolveSpeed;
            yield return null;
        }

    }
}
