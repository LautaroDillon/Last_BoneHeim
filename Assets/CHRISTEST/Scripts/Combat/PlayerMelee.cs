using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerMelee : MonoBehaviour
{
    [Header("Melee Settings")]
    public KeyCode meleeKey = KeyCode.F;
    public float meleeRange = 2f;
    public float meleeRadius = 1f;
    public int meleeDamage = 20;
    public LayerMask meleeHitMask;
    private float meleeCooldown = 0.5f;
    private float nextMeleeTime = 0f;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    [Header("Particle")]
    public GameObject enemyHitEffect;

    public void Update()
    {
        if (PauseManager.isPaused)
            return;
        else
        {
            HandleMeleeAttack();
        }
    }

    void HandleMeleeAttack()
    {
        if (Input.GetKeyDown(meleeKey) && Time.time >= nextMeleeTime)
        {
            nextMeleeTime = Time.time + meleeCooldown;
            PerformMeleeAttack();
            AudioManager.instance.PlaySFXOneShot("Melee", 1f);
        }
    }

    void PerformMeleeAttack()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Vector3 sphereCenter = origin + direction * meleeRange;

        Collider[] hits = Physics.OverlapSphere(sphereCenter, meleeRadius, meleeHitMask);

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out IDamagable damageable))
            {
                damageable.TakeDamage(meleeDamage);

                Vector3 knockbackDir = (hit.transform.position - transform.position).normalized;

                if (hit.attachedRigidbody != null)
                    hit.attachedRigidbody.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);

                Instantiate(enemyHitEffect, hit.ClosestPoint(transform.position), Quaternion.identity);
                AudioManager.instance.PlaySFXOneShot("MeleeImpact", 1f);
                CameraShake.Instance.ShakeOnce(4f, 4f, 0.1f, 0.5f);
                Debug.Log("Melee hit: " + hit.name);
                break;
            }
        }

        CameraShake.Instance.ShakeOnce(2f, 2f, 0.1f, 0.3f);
    }

    void OnDrawGizmosSelected()
    {
        if (meleeKey == KeyCode.F)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward * meleeRange, meleeRadius);
        }
    }
}
