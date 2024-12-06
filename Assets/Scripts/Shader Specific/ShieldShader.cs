using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldShader : MonoBehaviour, Idamagable
{
    [SerializeField]
    Renderer _renderer;
    [SerializeField] AnimationCurve _DisplacementCurve;
    [SerializeField] float _DisplacementMagnitude;
    [SerializeField] float _LerpSpeed;
    [SerializeField] float _DissolveSpeed;
    [SerializeField] public bool _shieldOn;

    Coroutine _dissolveCoroutine;

    public GameObject esto;
    public Collider _shieldCollider;
    public float _life;



    void Start()
    {
        _renderer = GetComponent<Renderer>();
        //_shieldOn = true;
    }


    public void HitShield(Vector3 hitpos)
    {
        _renderer.material.SetVector("_HitPosition", hitpos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());

    }

    public void OpenCloseShield()
    {
        float target = 1.3f;

        if (_shieldOn)
        {
            target = -0.2f;
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
        //if(_shieldOn == true)
        //{
        //    esto.SetActive(false);
        //}
    }

    public void TakeDamage(float dmg)
    {
        _life -= dmg;
        //HitShield();
        if (_life <= 0)
        {
            _shieldCollider.enabled = false;
            OpenCloseShield();
        }
    }
}
