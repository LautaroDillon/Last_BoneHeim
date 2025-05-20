using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class PlayerMelee : MonoBehaviour
{
    public PlayerMovement pm;
    public PlayerWeapon pw;
    public ThrowableObject throwable;
    private float initialSpeed;
    private float initialJump;

    [Header("Melee Settings")]
    public KeyCode meleeKey = KeyCode.F;
    public float meleeRange = 2f;
    public float meleeRadius = 1f;
    public int meleeDamage = 20;
    public LayerMask meleeHitMask;
    public bool canMelee = true;
    public float meleeCooldown = 3f;
    private float nextMeleeTime = 0f;
    private float recallDelay = 0.2f;
    private float timeThrown = 0f;

    [Header("Knockback")]
    public float knockbackForce = 5f;

    [Header("Throwing Settings")]
    public KeyCode throwKey = KeyCode.F;
    public GameObject throwableObjectPrefab;
    public Transform throwOrigin;
    public float throwForce = 30f;
    public float recallSpeed = 50f;
    public LayerMask hitMask;

    [Header("Throwing Cooldown Settings")]
    public float throwCooldown = 1.5f;
    private float nextThrowTime = 0f;

    [Header("Cooldown UI")]
    public Image meleeCooldownUI;
    public Image throwCooldownUI;

    private GameObject currentThrowable;
    private bool isHoldingToThrow = false;
    private bool isThrowableAway = false;
    private bool isRecalling = false;
    private bool hasPlayedRecallSound = false;

    [Header("Particle")]
    public GameObject enemyHitEffect;

    private void Start()
    {
        initialJump = pm.jumpForce;
        initialSpeed = pm.walkSpeed;
    }

    public void Update()
    {
        if (PauseManager.isPaused)
            return;

        if (!isThrowableAway)
        {
            if (Input.GetKeyDown(throwKey) && Time.time >= nextThrowTime)
            {
                isHoldingToThrow = true;
            }

            if (Input.GetKeyUp(throwKey) && isHoldingToThrow)
            {
                pm.walkSpeed = initialSpeed;
                pm.jumpForce = initialJump;
                ThrowObject();
                isHoldingToThrow = false;
            }

            if (!isHoldingToThrow && canMelee)
            {
                HandleMeleeAttack();
            }
        }
        else
        {
            if (Input.GetKeyDown(throwKey) && Time.time > timeThrown + recallDelay)
            {
                isRecalling = true;
            }
        }

        if (isRecalling && currentThrowable != null)
        {
            RecallObject();
        }

        if (meleeCooldownUI != null)
        {
            float meleeTimeLeft = Mathf.Clamp(nextMeleeTime - Time.time, 0, meleeCooldown);
            meleeCooldownUI.fillAmount = 1 - (meleeTimeLeft / meleeCooldown);
        }

        if (throwCooldownUI != null)
        {
            float throwTimeLeft = Mathf.Clamp(nextThrowTime - Time.time, 0, throwCooldown);
            throwCooldownUI.fillAmount = 1 - (throwTimeLeft / throwCooldown);
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

                if (((1 << hit.gameObject.layer) & meleeHitMask) != 0)
                {
                    Vector3 knockbackDir = (hit.transform.position - transform.position).normalized;

                    if (hit.attachedRigidbody != null)
                        hit.attachedRigidbody.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
                }

                Instantiate(enemyHitEffect, hit.ClosestPoint(transform.position), Quaternion.identity);
                AudioManager.instance.PlaySFXOneShot("MeleeImpact", 1f);
                CameraShake.Instance.ShakeOnce(4f, 4f, 0.1f, 0.5f);
                Debug.Log("Melee hit: " + hit.name);
                pw.currentAmmo += 5;
                break;
            }
        }

        CameraShake.Instance.ShakeOnce(2f, 2f, 0.1f, 0.3f);
    }

    void ThrowObject()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, hitMask))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(50f);

        Vector3 dir = (targetPoint - throwOrigin.position).normalized;

        if (currentThrowable != null)
        {
            Destroy(currentThrowable);
        }

        timeThrown = Time.time;

        currentThrowable = Instantiate(throwableObjectPrefab, throwOrigin.position, Quaternion.identity);
        Rigidbody rb = currentThrowable.GetComponent<Rigidbody>();
        rb.velocity = dir * throwForce;

        ThrowableObject throwableScript = currentThrowable.GetComponent<ThrowableObject>();
        if (throwableScript != null)
        {
            throwableScript.damage = meleeDamage;
            throwableScript.damageableLayer = meleeHitMask;
        }

        CameraShake.Instance.ShakeOnce(2f, 2f, 0.1f, 0.3f);
        pw.magazineSize = 5;
        isThrowableAway = true;
        canMelee = false;
        
    }

    void RecallObject()
    {
        if (currentThrowable == null) return;

        Vector3 direction = (throwOrigin.position - currentThrowable.transform.position).normalized;
        float distance = Vector3.Distance(currentThrowable.transform.position, throwOrigin.position);

        if (!hasPlayedRecallSound && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFXOneShot("ArmCall", 1f);
            hasPlayedRecallSound = true;
        }

        Rigidbody rb = currentThrowable.GetComponent<Rigidbody>();
        rb.velocity = direction * recallSpeed;

        currentThrowable.transform.Rotate(Vector3.up * 720 * Time.deltaTime);

        if (distance < 1f)
        {
            CameraShake.Instance.ShakeOnce(4f, 8f, 0.1f, 0.3f);
            AudioManager.instance.PlaySFXOneShot("ArmRecovery", 1f);

            Destroy(currentThrowable);
            pw.magazineSize = 10;
            pw.currentAmmo += 10;
            isThrowableAway = false;
            isRecalling = false;
            canMelee = true;
            hasPlayedRecallSound = false;
            nextThrowTime = Time.time + throwCooldown;
        }
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
