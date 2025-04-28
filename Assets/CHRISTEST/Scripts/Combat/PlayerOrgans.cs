using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrgans : MonoBehaviour, IInteractable
{
    private PlayerMovement pm;

    private void Awake()
    {
        pm = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public void Interact()
    {
        switch (gameObject.tag)
        {
            case "Heart":
                break;

            case "Lungs":
                pm.canDash = true;
                break;

            case "Stomach":
                pm.canDoubleJump = true;
                break;

            default:
                break;
        }

        Destroy(gameObject);
    }
}
