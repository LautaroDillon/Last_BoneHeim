using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class EventAnim : MonoBehaviour
{
    public static EventAnim instance;

    public GameObject organPoint;
    public GameObject organs;
    public GameObject heartPosition;

    private bool isFollowing = false;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private PlayerSlide playerSlide;
    private PlayerWeapon playerWeapon;

    private void Awake()
    {
        instance = this;
        organs.SetActive(false);
        heartPosition.SetActive(false);
        playerWeapon = FindObjectOfType<PlayerWeapon>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        playerSlide = FindAnyObjectByType<PlayerSlide>();
        playerDash = FindAnyObjectByType<PlayerDash>();
    }

    public void HeartEventActive()
    {
        organs.SetActive(true);
        playerMovement.enabled = false;
        playerDash.enabled = false;
        playerSlide.enabled = false;
        playerWeapon.enabled = false;
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        organs.transform.SetParent(null);
        organs.SetActive(false);
        heartPosition.SetActive(true);
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
