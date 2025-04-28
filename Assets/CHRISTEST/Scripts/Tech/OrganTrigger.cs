using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganTrigger : MonoBehaviour
{
    private PlayerMovement pm;

    private void Awake()
    {
        pm = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();

        if (pm != null)
        {
            pm.sprintSpeed = 6;
            pm.jumpForce = 0;
        }
        else
        {
            Debug.LogWarning("PlayerMovement component not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && pm != null)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Organ"))
            {
                switch (gameObject.tag)
                {
                    case "Heart":
                        pm.sprintSpeed = 12;
                        pm.jumpForce = 15;
                        PlaySound("Heartbeat");
                        break;

                    case "Lungs":
                        pm.canDash = true;
                        PlaySound("Lungs");
                        break;

                    case "Stomach":
                        pm.canDoubleJump = true;
                        PlaySound("Stomach");
                        break;
                }

                Destroy(gameObject); // Remove organ after pickup
            }
        }
    }

    private void PlaySound(string soundName)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFXOneShot(soundName, 1f);
        }
    }
}
