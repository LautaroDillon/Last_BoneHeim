using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Organs")]
    public PlayerMovement pm;
    public GameObject heart;
    public GameObject lungs;
    public GameObject stomach;

    private bool lastNormalSpeed;
    private bool lastCanDash;
    private bool lastCanDoubleJump;

    private void Awake()
    {
        heart.gameObject.SetActive(false);
        lungs.gameObject.SetActive(false);
        stomach.gameObject.SetActive(false);

        lastNormalSpeed = pm.normalSpeed;
        lastCanDash = pm.canDash;

        UIUpdate();
    }

    private void Update()
    {
        if (pm.normalSpeed != lastNormalSpeed || pm.canDash != lastCanDash || pm.canDoubleJump != lastCanDoubleJump)
        {
            UIUpdate();
            lastNormalSpeed = pm.normalSpeed;
            lastCanDash = pm.canDash;
            lastCanDoubleJump = pm.canDoubleJump;
        }
    }

    public void UIUpdate()
    {
        heart.gameObject.SetActive(pm.normalSpeed);
        lungs.gameObject.SetActive(pm.canDash);
        stomach.gameObject.SetActive(pm.canDoubleJump);
    }

    public void isselected()
    {
        /*if (HotbarPlayer.Instance.)
        {

        }*/
    }
}
