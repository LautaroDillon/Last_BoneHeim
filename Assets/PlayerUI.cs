using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Organs")]
    public PlayerMovement pm;
    public Image heart;
    public Image lungs;

    private bool lastNormalSpeed;
    private bool lastCanDash;

    private void Awake()
    {
        heart.gameObject.SetActive(false);
        lungs.gameObject.SetActive(false);

        lastNormalSpeed = pm.normalSpeed;
        lastCanDash = pm.canDash;

        UIUpdate();
    }

    private void Update()
    {
        if (pm.normalSpeed != lastNormalSpeed || pm.canDash != lastCanDash)
        {
            UIUpdate();
            lastNormalSpeed = pm.normalSpeed;
            lastCanDash = pm.canDash;
        }
    }

    public void UIUpdate()
    {
        heart.gameObject.SetActive(pm.normalSpeed);
        lungs.gameObject.SetActive(pm.canDash);
    }
}
