using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public GameObject player;
    private Vector3 respawnPoint;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        respawnPoint = newCheckpoint;
    }

    public void Respawn(GameObject player)
    {
        AudioManager.instance.PlayMusic("Background Music", 1f);
        DeathUI.deathUiActive = false;

        var rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.MovePosition(respawnPoint);
        }

        var cam = player.GetComponentInChildren<Camera>();
        if (cam != null)
            cam.enabled = true;

        PlayerHealth.hasDied = false;

        var deathCam = GameObject.Find("DeathCamera(Clone)");
        if (deathCam != null)
            Destroy(deathCam);
    }
}
