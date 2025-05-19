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

    private PlayerWeapon playerWeapon;

    private void Awake()
    {
        instance = this;
        organs.SetActive(false);
        heartPosition.SetActive(false);
        playerWeapon = FindObjectOfType<PlayerWeapon>();
    }

    public void HeartEventActive()
    {
        organs.SetActive(true);
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        organs.transform.SetParent(null);
        organs.SetActive(false);
        heartPosition.SetActive(true);
    }

    [Preserve]
    public void ShootingEvent()
    {
        Debug.LogWarning("Shooting Event");
        playerWeapon.FireBulletFromAnimation();
    }
}
