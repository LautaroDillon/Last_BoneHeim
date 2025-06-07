using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrganGrenade : MonoBehaviour
{
    [Header("Settings")]
    public float delay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float damage = 50f;
    public LayerMask damageMask;

    [Header("Effects")]
    public GameObject explosionVFX;
    public AudioClip explosionSFX;

    protected bool hasExploded = false;

    protected virtual void Start()
    {
        Invoke(nameof(Explode), delay);
    }

    protected virtual void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionVFX)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        if (explosionSFX)
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position);

        DoExplosion();

        Destroy(gameObject);
    }

    protected abstract void DoExplosion();
}
