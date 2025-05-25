using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationForwarder : MonoBehaviour
{
    public PlayerWeapon playerWeapon;

    public void FireBulletFromAnimation()
    {
        if (playerWeapon == null)
            playerWeapon = GetComponentInParent<PlayerWeapon>();

        if (playerWeapon != null)
            playerWeapon.FireBulletFromAnimation();
        else
            Debug.LogWarning("PlayerWeapon reference not found!");
    }
}
