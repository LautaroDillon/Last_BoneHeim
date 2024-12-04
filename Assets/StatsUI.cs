using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI health;
    [SerializeField] TextMeshProUGUI speed;
    [SerializeField] TextMeshProUGUI jumpForce;
    [SerializeField] TextMeshProUGUI jumpAirMulti;
    [SerializeField] TextMeshProUGUI slideSpeed;
    [SerializeField] TextMeshProUGUI wallrunningSpeed;
    [SerializeField] TextMeshProUGUI additionalReviveTimer;
    [SerializeField] TextMeshProUGUI additionalDamage;
    [SerializeField] TextMeshProUGUI shieldAmount;
    [SerializeField] TextMeshProUGUI lifestealAmount;

    private void Update()
    {
        health.text = "Health: " + PlayerHealth.instance._maxlife;
        speed.text = "Speed: " + PlayerMovementAdvanced.instance.sprintSpeed;
        jumpForce.text = "Jump Force: " + PlayerMovementAdvanced.instance.jumpForce;
        jumpAirMulti.text = "Jump Multiplier: " + PlayerMovementAdvanced.instance.airMultiplier;
        slideSpeed.text = "Slide Speed: " + PlayerMovementAdvanced.instance.slideSpeed;
        wallrunningSpeed.text = "Wallrunning Speed: " + PlayerMovementAdvanced.instance.wallrunSpeed;
        additionalReviveTimer.text = "Revive Time: " + PlayerHealth.instance.reviveTime;
        additionalDamage.text = "Damage: " + FlyweightPointer.Player.Damage;
        shieldAmount.text = "Shield Amount: " + PlayerHealth.instance.shieldAmount;
        lifestealAmount.text = "Lifesteal: " + PlayerHealth.instance.lifeSteal;
    }
}
