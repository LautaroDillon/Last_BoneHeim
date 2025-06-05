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

    [Header("Materials Organs")]
    public GameObject heartMaterial;
    public GameObject lungsMaterial;
    public GameObject stomachMaterial;

    public bool lastNormalSpeed;
    public bool lastCanDash;
    public bool lastCanDoubleJump;

    [Header("UI Inventory")]
    public KeyCode OpenUI = KeyCode.Tab;
    public GameObject UI_canvasInv;
    public bool IsOpenUI;

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

        UI_canvasInv.SetActive(false);
    }

    private void Update()
    {
        if (pm.normalSpeed != lastNormalSpeed || pm.canDash != lastCanDash || pm.canDoubleJump != lastCanDoubleJump)
        {
            UIUpdate();
        }

        Inventory();
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
                heart.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                lungs.transform.localScale = new Vector3(1f, 1f, 1f);
                stomach.transform.localScale = new Vector3(1f, 1f, 1f);

                break;
            case "Lungs":
                lungs.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                heart.transform.localScale = new Vector3(1f, 1f, 1f);
                stomach.transform.localScale = new Vector3(1f, 1f, 1f);

                break;
            case "Stomach":
                stomach.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                heart.transform.localScale = new Vector3(1f, 1f, 1f);
                lungs.transform.localScale = new Vector3(1f, 1f, 1f);

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
            case "O_Lungs":
                pm.canDash = false;
                break;
            case "O_Stomach":
                pm.canDoubleJump = false;
                break;
        }
    }

    public void Inventory()
    {
        if (Input.GetKeyDown(OpenUI) && !IsOpenUI)
        {
            IsOpenUI = true;
            UI_canvasInv.SetActive(true);

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyDown(OpenUI) && IsOpenUI)
        {
            IsOpenUI = false;
            UI_canvasInv.SetActive(false);

            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


}
