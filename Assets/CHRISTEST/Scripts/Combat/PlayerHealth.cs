using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using EZCameraShake;

public class PlayerHealth : Entity
{
    public static bool hasDied = false;

    [Header("Death")]
    public DeathUI deathUI;
    public Camera mainCam;
    public GameObject deathCamPrefab;

    private void Awake()
    {
        hasDied = false;
        isDead = false;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        mainCam = Camera.main;
    }

    private void Update()
    {
        Death();
    }

    private void Death()
    {
        if (isDead && !hasDied)
        {
            hasDied = true;
            AudioManager.instance.PlayMusic("Death Music", 0.5f);
            DeathCamera();
            deathUI.TriggerDeathFade();
        }
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);
        CameraShake.Instance.ShakeOnce(4f, 4f, 0.1f, 1f);
        FullscreenShader.instance.hitShaderEnabled = true;
    }

    public void Respawn()
    {
        currentHealth = maxHealth;
        hasDied = false;
        isDead = false;
        CheckpointManager.instance.Respawn(GameObject.FindWithTag("Player"));
        deathUI.TriggerDeathFadeOut();
    }

    private void DeathCamera()
    {
        mainCam.enabled = false;

        GameObject deathCam = Instantiate(deathCamPrefab, mainCam.transform.position, mainCam.transform.rotation);

        Rigidbody camRb = deathCam.GetComponent<Rigidbody>();

        camRb.isKinematic = false;
        camRb.useGravity = true;
        camRb.AddForce(Vector3.back * 5f + Vector3.up * 2f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "EnemyBullet")
        {
            TakeDamage(EnemyFlyweight.Shooter.Damage);
            AudioManager.instance.PlaySFX("Player Bullet Impact", 1f, false);
        }
    }
}
