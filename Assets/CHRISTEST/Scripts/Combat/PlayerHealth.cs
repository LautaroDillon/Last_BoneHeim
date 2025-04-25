using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

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
        mainCam = Camera.main;
    }

    private void Update()
    {
        Death();
        //if (Input.GetKeyDown(KeyCode.R))
            //Respawn();
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
}
