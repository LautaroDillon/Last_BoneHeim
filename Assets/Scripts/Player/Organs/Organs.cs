using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organs : MonoBehaviour
{
    public static Organs instance;
    #region Normal Organs
    [Header("Organs")]
    //Heart
    [SerializeField] float heartLifeBuff = 10;

    //Lungs
    [SerializeField] float lungsSprintBuff = 2;
    [SerializeField] float lungsSlideBuff = 0;
    [SerializeField] float lungsClimbBuff = 0;
    [SerializeField] float lungsWallrunBuff = 0;

    //Stomach
    [SerializeField] float stomachJumpBuff = 2;
    [SerializeField] float stomachAirBuff = 0;

    //Liver
    [SerializeField] float liverDamageBuff = 10;
    [SerializeField] float liverBoneBuff = 2;
    #endregion

    #region Cursed Organs
    [Header("Cursed Organs")]
    //Heart
    [SerializeField] float cursedHeartLifeDebuff = 10;
    [SerializeField] float cursedHeartReviveBuff = 3;

    //Lungs
    [SerializeField] float cursedLungsSprintBuff = 3;
    [SerializeField] float cursedLungsSlideBuff = 3;
    [SerializeField] float cursedLungsClimbBuff = 3;
    [SerializeField] float cursedLungsWallrunBuff = 3;

    //Stomach
    [SerializeField] float cursedStomachJumpBuff = 4;
    [SerializeField] float cursedStomachAirBuff = 1;

    //Liver
    [SerializeField] float cursedLiverDamageBuff = 20;
    [SerializeField] float cursedLiverBoneBuff = 4;
    #endregion

    #region Blazing Organs
    [Header("Blazing Organs")]
    //Heart
    [SerializeField] float blazingHeartLifeBuff = 20;

    //Lungs
    [SerializeField] float blazingLungsSprintBuff = 3;
    [SerializeField] float blazingLungsSlideBuff = 1;
    [SerializeField] float blazingLungsClimbBuff = 1;
    [SerializeField] float blazingLungsWallrunBuff = 0;

    //Stomach
    [SerializeField] float blazingStomachJumpBuff = 3;
    [SerializeField] float blazingStomachAirBuff = 0.5f;

    //Liver
    [SerializeField] float blazingLiverDamageBuff = 15;
    [SerializeField] float blazingLiverBoneBuff = 3;
    #endregion

    [Header("Frostbit Organs")] //can freeze or slow down enemies

    [Header("Soulbound Organs")] //spawns allies for combat

    [Header("Spectral Organs")] //fast movement, can dodge bullets

    [Header("Divine Organs")] //op in everything

    [Header("Vengeful Organs")] //lifesteal and return damage

    [Header("Blessed Organs")]
    int random;

    #region Equip Organ

    private void Awake()
    {
        instance = this;
    }

    public void EquipHeart()
    {
        Debug.Log("HEART WORKS");
        PlayerHealth.instance._maxlife += heartLifeBuff;
    }

    public void EquipCursedHeart()
    {
        PlayerHealth.instance._maxlife -= cursedHeartLifeDebuff;
        PlayerHealth.instance.reviveTime += cursedHeartReviveBuff;
    }
    public void EquipBlazingHeart()
    {
        PlayerHealth.instance._maxlife += blazingHeartLifeBuff;
    }

    public void EquipLungs()
    {
        PlayerMovementAdvanced.instance.sprintSpeed += lungsSprintBuff;
        PlayerMovementAdvanced.instance.climbSpeed += lungsClimbBuff;
        PlayerMovementAdvanced.instance.slideSpeed += lungsSlideBuff;
        PlayerMovementAdvanced.instance.wallrunSpeed += lungsWallrunBuff;
    }

    public void EquipCursedLungs()
    {
        PlayerHealth.instance._maxlife -= cursedHeartLifeDebuff;

        PlayerMovementAdvanced.instance.sprintSpeed += cursedLungsSprintBuff;
        PlayerMovementAdvanced.instance.climbSpeed += cursedLungsClimbBuff;
        PlayerMovementAdvanced.instance.slideSpeed += cursedLungsSlideBuff;
        PlayerMovementAdvanced.instance.wallrunSpeed += cursedLungsWallrunBuff;
    }
    public void EquipBlazingLungs()
    {
        PlayerMovementAdvanced.instance.sprintSpeed += blazingLungsSprintBuff;
        PlayerMovementAdvanced.instance.climbSpeed += blazingLungsClimbBuff;
        PlayerMovementAdvanced.instance.slideSpeed += blazingLungsSlideBuff;
        PlayerMovementAdvanced.instance.wallrunSpeed += blazingLungsWallrunBuff;
    }

    public void EquipStomach()
    {
        PlayerMovementAdvanced.instance.jumpForce += stomachJumpBuff;
        PlayerMovementAdvanced.instance.airMultiplier += stomachAirBuff;
    }

    public void EquipCursedStomach()
    {
        PlayerHealth.instance._maxlife -= cursedHeartLifeDebuff;

        PlayerMovementAdvanced.instance.jumpForce += cursedStomachJumpBuff;
        PlayerMovementAdvanced.instance.airMultiplier += cursedStomachAirBuff;
    }
    public void EquipBlazingStomach()
    {
        PlayerMovementAdvanced.instance.jumpForce += blazingStomachJumpBuff;
        PlayerMovementAdvanced.instance.airMultiplier += blazingStomachAirBuff;
    }

    public void EquipLiver()
    {
        FlyweightPointer.Player.Damage += liverDamageBuff;
        Guns.instance.killReward += liverBoneBuff;
    }

    public void EquipCursedLiver()
    {
        PlayerHealth.instance._maxlife -= cursedHeartLifeDebuff;

        FlyweightPointer.Player.Damage += cursedLiverDamageBuff;
        Guns.instance.killReward += cursedLiverBoneBuff;
    }
    public void EquipBlazingLiver()
    {
        FlyweightPointer.Player.Damage += blazingLiverDamageBuff;
        Guns.instance.killReward += blazingLiverBoneBuff;
    }

    #endregion

    #region Unequip Organ

    public void UnequipHeart()
    {
        Debug.Log("HEART DOESNT WORK");
        PlayerHealth.instance._maxlife -= heartLifeBuff;
    }

    public void UnequipCursedHeart()
    {
        PlayerHealth.instance._maxlife += cursedHeartLifeDebuff;
        PlayerHealth.instance.reviveTime -= cursedHeartReviveBuff;
    }
    public void UnequipBlazingHeart()
    {
        PlayerHealth.instance._maxlife -= blazingHeartLifeBuff;
    }

    public void UnequipLungs()
    {
        PlayerMovementAdvanced.instance.sprintSpeed -= lungsSprintBuff;
        PlayerMovementAdvanced.instance.climbSpeed -= lungsClimbBuff;
        PlayerMovementAdvanced.instance.slideSpeed -= lungsSlideBuff;
        PlayerMovementAdvanced.instance.wallrunSpeed -= lungsWallrunBuff;
    }

    public void UnequipCursedLungs()
    {
        PlayerHealth.instance._maxlife += cursedHeartLifeDebuff;

        PlayerMovementAdvanced.instance.sprintSpeed -= cursedLungsSprintBuff;
        PlayerMovementAdvanced.instance.climbSpeed -= cursedLungsClimbBuff;
        PlayerMovementAdvanced.instance.slideSpeed -= cursedLungsSlideBuff;
        PlayerMovementAdvanced.instance.wallrunSpeed -= cursedLungsWallrunBuff;
    }
    public void UnequipBlazingLungs()
    {
        PlayerMovementAdvanced.instance.sprintSpeed -= blazingLungsSprintBuff;
        PlayerMovementAdvanced.instance.climbSpeed -= blazingLungsClimbBuff;
        PlayerMovementAdvanced.instance.slideSpeed -= blazingLungsSlideBuff;
        PlayerMovementAdvanced.instance.wallrunSpeed -= blazingLungsWallrunBuff;
    }

    public void UnequipStomach()
    {
        PlayerMovementAdvanced.instance.jumpForce -= stomachJumpBuff;
        PlayerMovementAdvanced.instance.airMultiplier -= stomachAirBuff;
    }

    public void UnequipCursedStomach()
    {
        PlayerHealth.instance._maxlife += cursedHeartLifeDebuff;

        PlayerMovementAdvanced.instance.jumpForce -= cursedStomachJumpBuff;
        PlayerMovementAdvanced.instance.airMultiplier -= cursedStomachAirBuff;
    }
    public void UnequipBlazingStomach()
    {
        PlayerMovementAdvanced.instance.jumpForce -= blazingStomachJumpBuff;
        PlayerMovementAdvanced.instance.airMultiplier -= blazingStomachAirBuff;
    }

    public void UnequipLiver()
    {
        FlyweightPointer.Player.Damage -= liverDamageBuff;
        Guns.instance.killReward -= liverBoneBuff;
    }

    public void UnequipCursedLiver()
    {
        PlayerHealth.instance._maxlife += cursedHeartLifeDebuff;

        FlyweightPointer.Player.Damage -= liverDamageBuff;
        Guns.instance.killReward -= cursedLiverBoneBuff;
    }
    public void UnequipBlazingLiver()
    {
        FlyweightPointer.Player.Damage -= blazingLiverDamageBuff;
        Guns.instance.killReward -= blazingLiverBoneBuff;
    }

    #endregion
}
