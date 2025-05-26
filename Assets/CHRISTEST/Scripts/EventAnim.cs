using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class EventAnim : MonoBehaviour
{
    public static EventAnim instance;

    public GameObject organPoint;

    public GameObject heartOrgan;
    public GameObject lungsOrgan;
    public GameObject stomachOrgan;

    public GameObject heartPosition;
    public GameObject lungsPosition;
    public GameObject stomachPosition;

    private bool isFollowing = false;
    private PlayerMovement playerMovement;
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
        heartOrgan.SetActive(true);
        playerMovement.enabled = false;
        playerDash.enabled = false;
        playerSlide.enabled = false;
        playerWeapon.enabled = false;
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        heartOrgan.transform.SetParent(null);
        heartOrgan.SetActive(false);
        heartPosition.SetActive(true);
        playerMovement.enabled = true;
        playerDash.enabled = true;
        playerSlide.enabled = true;
        playerWeapon.enabled = true;
    }

    public void StomachEventActive()
    {
        stomachOrgan.SetActive(true);
        playerMovement.enabled = false;
        playerDash.enabled = false;
        playerSlide.enabled = false;
        playerWeapon.enabled = false;
    }

    public void StomachEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        stomachOrgan.transform.SetParent(null);
        stomachOrgan.SetActive(false);
        stomachPosition.SetActive(true);
        playerMovement.enabled = true;
        playerDash.enabled = true;
        playerSlide.enabled = true;
        playerWeapon.enabled = true;
    }

    public void LungsEventActive()
    {
        lungsOrgan.SetActive(true);
        playerMovement.enabled = false;
        playerDash.enabled = false;
        playerSlide.enabled = false;
        playerWeapon.enabled = false;
    }

    public void lungsEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        lungsOrgan.transform.SetParent(null);
        lungsOrgan.SetActive(false);
        lungsPosition.SetActive(true);
        playerMovement.enabled = true;
        playerDash.enabled = true;
        playerSlide.enabled = true;
        playerWeapon.enabled = true;
    }

    [Preserve]
    public void ShootingEvent()
    {
        Debug.LogWarning("Shooting Event");
        playerWeapon.FireBulletFromAnimation();
    }
}
