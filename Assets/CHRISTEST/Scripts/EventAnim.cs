using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class EventAnim : MonoBehaviour
{
    public static EventAnim instance;

    public GameObject organPoint;

    [Header("Organs")]
    public GameObject heartOrgan;
    public GameObject lungsOrgan;
    public GameObject stomachOrgan;

    [Header("Organ Positions")]
    public GameObject heartPosition;
    public GameObject lungsPosition;
    public GameObject stomachPosition;



    private bool isFollowing = false;
    [Header("Player")]
    public PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private PlayerSlide playerSlide;
    private PlayerWeapon playerWeapon;

    private void Awake()
    {
        instance = this;
        heartOrgan.SetActive(false);
        heartPosition.SetActive(false);

        lungsOrgan.SetActive(false);
        lungsPosition.SetActive(false);

        stomachOrgan.SetActive(false);
        stomachPosition.SetActive(false);

        playerWeapon = FindObjectOfType<PlayerWeapon>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerSlide = FindAnyObjectByType<PlayerSlide>();
        playerDash = FindAnyObjectByType<PlayerDash>();
    }

    public void HeartEventActive()
    {
        playerMovement.enabled = false;
        playerDash.enabled = false;
        playerSlide.enabled = false;
        playerWeapon.enabled = false;
        PlayerMovement.instance.whatorgan.SetActive(false);
        if (playerMovement.tagname == "Heart")
        {
            heartOrgan.SetActive(true);
            PlayerUI.instance.lastNormalSpeed = true;
        }
        else if (playerMovement.tagname == "Stomach")
        {
            stomachOrgan.SetActive(true);
            PlayerUI.instance.lastCanDoubleJump = true;
        }
        else if (playerMovement.tagname == "Lungs")
        {
            lungsOrgan.SetActive(true);
            PlayerUI.instance.lastCanDash = true;
        }
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        playerMovement.freeze = false;
        isFollowing = false;
        //heartOrgan.transform.SetParent(null);
        //desactivate the organs
        heartOrgan.SetActive(false);
        lungsOrgan.SetActive(false);
        stomachOrgan.SetActive(false);

        if (playerMovement.tagname == "Heart")
        {
            heartPosition.SetActive(true);
        }
        else if (playerMovement.tagname == "Stomach")
        {
            stomachPosition.SetActive(true);
        }
        else if (playerMovement.tagname == "Lungs")
        {
            lungsPosition.SetActive(true);
        }

        playerMovement.enabled = true;
        playerDash.enabled = true;
        playerSlide.enabled = true;
        playerWeapon.enabled = true;
        FindObjectOfType<ColorRestorer>().RestoreColor();
    }

    [Preserve]
    public void ShootingEvent()
    {
        Debug.LogWarning("Shooting Event");
        playerWeapon.FireBulletFromAnimation();
    }
}
