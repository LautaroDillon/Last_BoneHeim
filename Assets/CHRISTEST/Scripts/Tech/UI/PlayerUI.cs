using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;

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
    private void Start()
    {
        instance = this;
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

    public void isSelected(string organtype)
    {
        switch (organtype)
        {
            case "Heart":

                break;
            case "Lungs":

                break;
            case "Stomach":

                break;
        }
    }


    public void isUsed(string organtype)
    {
        switch (organtype)
        {
            case "Heart":
                pm.normalSpeed = true;
                break;
            case "Lungs":
                pm.canDash = false;
                break;
            case "Stomach":
                pm.canDoubleJump = false;
                break;
        }
    }
}
