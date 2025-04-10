using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    int forgottenBonus = 0;
    int forgottenHeartStat;
    int forgottenLiverStat;
    int forgottenStomachStat;
    int forgottenLungsStat;
    [Header("References")]

    public InventoryObject inventory;
    public InventoryObject equipment;
    public Attribute[] attributes;

    [Header("Sounds")]

    //Feedback
    [SerializeField] private AudioClip groundPickUpClip;
    [SerializeField] private AudioClip equipmentClip;
    [SerializeField] private AudioClip unequipClip;

    //Types
    [SerializeField] protected AudioClip blazingOrganClip;
    [SerializeField] protected AudioClip cursedOrganClip;
    [SerializeField] protected AudioClip blessedOrganClip;
    [SerializeField] protected AudioClip vengefulOrganClip;

    //Organ Sounds
    [SerializeField] protected AudioClip heartEquipClip;
    [SerializeField] protected AudioClip lungsEquipClip;
    [SerializeField] protected AudioClip liverEquipClip;
    [SerializeField] protected AudioClip stomachEquipClip;
    [SerializeField] protected AudioClip stomachSecondaryClip;

    private void Awake()
    {
        inventory.Clear();
        equipment.Clear();
        forgottenHeartStat = Random.Range(-15, 25);
        forgottenLiverStat = Random.Range(-15, 15);
        forgottenStomachStat = Random.Range(-4, 4);
        forgottenLungsStat = Random.Range(-5, 5);
    }

    private void Start()
    {
        for (int i = 0; i < attributes.Length; i++)
        {
            attributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnBeforeSlotUpdate;
            equipment.GetSlots[i].OnAfterUpdate += OnAfterSlotUpdate;
        }
    }

    public void OnBeforeSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Removed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));
                SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                switch (_slot.ItemObject.type)
                {
                    #region Normal Organs
                    case ItemType.O_Heart:
                        Debug.Log("HEART DOESNT WORK");
                        PlayerHealth.instance._maxlife -= 20;
                        break;

                    case ItemType.O_Liver:
                        Debug.Log("LIVER DOESNT WORK");
                        FlyweightPointer.Player.Damage -= 5;
                        break;

                    case ItemType.O_Lungs:
                        Debug.Log("LUNGS DOESNT WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= 2;
                        break;

                    case ItemType.O_Stomach:
                        Debug.Log("STOMACH DOESNT WORK");
                        PlayerMovementAdvanced.instance.jumpForce -= 3;
                        break;
                    #endregion

                    #region Blazing Organs
                    case ItemType.O_BlazingHeart:
                        Debug.Log("BLAZING HEART DOESNT WORK");
                        PlayerHealth.instance._maxlife -= 10;
                        break;

                    case ItemType.O_BlazingLiver:
                        Debug.Log("BLAZING LIVER DOESNT WORK");
                        FlyweightPointer.Player.Damage -= 10;
                        break;

                    case ItemType.O_BlazingLungs:
                        Debug.Log("BLAZING LUNGS DOESNT WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 3;
                        break;

                    case ItemType.O_BlazingStomach:
                        Debug.Log("BLAZING STOMACH DOESNT WORK");
                        PlayerMovementAdvanced.instance.jumpForce -= 1;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        break;
                    #endregion

                    #region Cursed Organs
                    case ItemType.O_CursedHeart:
                        Debug.Log("CURSED HEART DOESNT WORK");
                        PlayerHealth.instance._maxlife += 10;
                        PlayerHealth.instance.reviveTime -= 3;
                        break;

                    case ItemType.O_CursedLiver:
                        Debug.Log("CURSED LIVER DOESNT WORK");
                        PlayerHealth.instance._maxlife += 10;
                        FlyweightPointer.Player.Damage -= 20;
                        break;

                    case ItemType.O_CursedLungs:
                        Debug.Log("CURSED LUNGS DOESNT WORK");
                        PlayerHealth.instance._maxlife += 10;
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 1;
                        PlayerMovementAdvanced.instance.climbSpeed -= 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 2;
                        break;

                    case ItemType.O_CursedStomach:
                        Debug.Log("CURSED STOMACH DOESNT WORK");
                        PlayerHealth.instance._maxlife += 10;
                        PlayerMovementAdvanced.instance.jumpForce -= 3;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        break;
                    #endregion

                    #region Vengeful Organs
                    case ItemType.O_VengefulHeart:
                        Debug.Log("VENGEFUL HEART DOESNT WORK");
                        PlayerHealth.instance._maxlife -= 15;
                        PlayerHealth.instance.lifeSteal -= 2;
                        break;

                    case ItemType.O_VengefulLiver:
                        Debug.Log("VENGEFUL LIVER DOESNT WORK");
                        PlayerHealth.instance.lifeSteal -= 2;
                        FlyweightPointer.Player.Damage -= 5;
                        break;

                    case ItemType.O_VengefulLungs:
                        Debug.Log("VENGEFUL LUNGS DOESNT WORK");
                        PlayerHealth.instance.lifeSteal -= 2;
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 1;
                        PlayerMovementAdvanced.instance.climbSpeed -= 1;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 1;
                        break;

                    case ItemType.O_VengefulStomach:
                        Debug.Log("VENGEFUL STOMACH DOESNT WORK");
                        PlayerHealth.instance.lifeSteal -= 2;
                        PlayerMovementAdvanced.instance.jumpForce -= 1;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        break;
                    #endregion

                    #region Blessed Organs
                    case ItemType.O_BlessedHeart:
                        Debug.Log("BLESSED HEART DOESNT WORK");
                        PlayerHealth.instance._maxlife -= 25;
                       /* PlayerHealth.instance.shieldAmount -= 25;
                        PlayerHealth.instance.shieldMax -= 25;*/
                        break;

                    case ItemType.O_BlessedLiver:
                        Debug.Log("BLESSED LIVER DOESNT WORK");
                       /* PlayerHealth.instance.shieldAmount -= 25;
                        PlayerHealth.instance.shieldMax -= 25;*/
                        FlyweightPointer.Player.Damage -= 10;
                        break;

                    case ItemType.O_BlessedLungs:
                        Debug.Log("BLESSED LUNGS DOESNT WORK");
                       /* PlayerHealth.instance.shieldAmount -= 25;
                        PlayerHealth.instance.shieldMax -= 25;*/
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 1;
                        PlayerMovementAdvanced.instance.climbSpeed -= 1;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 1;
                        break;

                    case ItemType.O_BlessedStomach:
                        Debug.Log("BLESSED STOMACH DOESNT WORK");
                       /* PlayerHealth.instance.shieldAmount -= 25;
                        PlayerHealth.instance.shieldMax -= 25;*/
                        PlayerMovementAdvanced.instance.jumpForce -= 1;
                        PlayerMovementAdvanced.instance.airMultiplier -= 2;
                        break;
                    #endregion

                    #region Forgotten Organs

                    case ItemType.O_ForgottenHeart:
                        Debug.Log("FORGOTTEN HEART WORKS");
                        PlayerHealth.instance._maxlife -= forgottenHeartStat;
                        forgottenBonus -= 5;
                        break;

                    case ItemType.O_ForgottenLungs:
                        Debug.Log("FORGOTTEN LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= forgottenLungsStat - forgottenBonus;
                        PlayerMovementAdvanced.instance.slideSpeed -= forgottenLungsStat - forgottenBonus;
                        PlayerMovementAdvanced.instance.climbSpeed -= forgottenLungsStat - forgottenBonus;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= forgottenLungsStat - forgottenBonus;
                        forgottenBonus -= 5;
                        break;

                    case ItemType.O_ForgottenLiver:
                        Debug.Log("FORGOTTEN LIVER WORKS");
                        FlyweightPointer.Player.Damage -= forgottenLiverStat;
                        forgottenBonus -= 5;
                        break;

                    case ItemType.O_ForgottenStomach:
                        Debug.Log("FORGOTTEN STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce -= forgottenStomachStat;
                        PlayerMovementAdvanced.instance.airMultiplier -= forgottenStomachStat;
                        forgottenBonus -= 5;
                        break;

                    #endregion

                    #region Spectral Organs

                    case ItemType.O_SpectralHeart:
                        Debug.Log("SPECTRAL HEART WORKS");
                        PlayerHealth.instance._maxlife -= 15;
                        PlayerHealth.instance.dodgeChance -= 14;
                        break;

                    case ItemType.O_SpectralLungs:
                        Debug.Log("SPECTRAL LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= 2;
                        PlayerMovementAdvanced.instance.slideSpeed -= 2;
                        PlayerMovementAdvanced.instance.climbSpeed -= 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 2;
                        PlayerHealth.instance.dodgeChance -= 7;
                        break;

                    case ItemType.O_SpectralLiver:
                        Debug.Log("SPECTRAL LIVER WORKS");
                        FlyweightPointer.Player.Damage -= 15;
                        PlayerHealth.instance.dodgeChance -= 7;
                        break;

                    case ItemType.O_SpectralStomach:
                        Debug.Log("SPECTRAL STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce -= 1;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        PlayerHealth.instance.dodgeChance -= 7;
                        break;

                    #endregion

                    #region Lightning Organs

                    case ItemType.O_LightningHeart:
                        Debug.Log("LIGHTNING HEART WORKS");
                        PlayerHealth.instance._maxlife -= 35;
                        Guns.instance.lightningDamage -= 5;
                        Guns.instance.lightningChance -= 7;
                        break;

                    case ItemType.O_LightningLungs:
                        Debug.Log("LIGHTNING LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= 3;
                        PlayerMovementAdvanced.instance.slideSpeed -= 3;
                        PlayerMovementAdvanced.instance.climbSpeed -= 1;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 1;
                        Guns.instance.lightningDamage -= 5;
                        Guns.instance.lightningChance -= 7;
                        break;

                    case ItemType.O_LightningLiver:
                        Debug.Log("LIGHTNING LIVER WORKS");
                        FlyweightPointer.Player.Damage -= 15;
                        Guns.instance.lightningDamage -= 5;
                        Guns.instance.lightningChance -= 7;
                        break;

                    case ItemType.O_LightningStomach:
                        Debug.Log("LIGHTING STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce -= 1;
                        PlayerMovementAdvanced.instance.airMultiplier -= 2;
                        Guns.instance.lightningDamage -= 5;
                        Guns.instance.lightningChance -= 7;
                        break;

                    #endregion

                    #region Infernal Organs

                    case ItemType.O_InfernalHeart:
                        Debug.Log("INFERNAL HEART WORKS");
                        PlayerHealth.instance._maxlife -= 10;
                        Guns.instance.infernalDamage -= 5;
                        Guns.instance.infernalChance -= 7;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_InfernalLungs:
                        Debug.Log("INFERNAL LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 1;
                        PlayerMovementAdvanced.instance.climbSpeed -= 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 2;
                        Guns.instance.infernalDamage -= 5;
                        Guns.instance.infernalChance -= 7;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_InfernalLiver:
                        Debug.Log("INFERNAL LIVER WORKS");
                        FlyweightPointer.Player.Damage -= 25;
                        Guns.instance.infernalDamage -= 5;
                        Guns.instance.infernalChance -= 7;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_InfernalStomach:
                        Debug.Log("INFERNAL STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce -= 2;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        Guns.instance.infernalDamage -= 5;
                        Guns.instance.infernalChance -= 7;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    #endregion

                    #region Rotten Organs

                    case ItemType.O_RottenHeart:
                        Debug.Log("ROTTEN HEART WORKS");
                        PlayerHealth.instance._maxlife -= 10;
                        Guns.instance.rottenDamage -= 5;
                        Guns.instance.rottenChance -= 7;
                        break;

                    case ItemType.O_RottenLungs:
                        Debug.Log("ROTTEN LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 1;
                        PlayerMovementAdvanced.instance.climbSpeed -= 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 2;
                        Guns.instance.rottenDamage -= 5;
                        Guns.instance.rottenChance -= 7;
                        break;

                    case ItemType.O_RottenLiver:
                        Debug.Log("ROTTEN LIVER WORKS");
                        FlyweightPointer.Player.Damage -= 25;
                        Guns.instance.rottenDamage -= 5;
                        Guns.instance.rottenChance -= 7;
                        break;

                    case ItemType.O_RottenStomach:
                        Debug.Log("ROTTEN STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce -= 2;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        Guns.instance.rottenDamage -= 5;
                        Guns.instance.rottenChance -= 7;
                        break;

                    #endregion

                    #region Hands
                    case ItemType.H_Skeleton:
                        Debug.Log("SKELETON HAND DOESNT WORK");
                        Guns.instance.ResetGun();
                        break;

                    case ItemType.H_Knuckle:
                        Debug.Log("KNUCKLE BUSTER DOESNT WORK");
                        Guns.instance.ResetGun();
                        break;

                    case ItemType.H_Invoker:
                        Debug.Log("INVOKER HAND DOESNT WORK");
                        Guns.instance.ResetGun();
                        break;

                    case ItemType.H_Teeth:
                        Debug.Log("TEETH SHOT DOESNT WORK");
                        Guns.instance.ResetGun();
                        break;

                    case ItemType.H_Nail:
                        Debug.Log("NAIL HAND DOESNT WORK");
                        Guns.instance.ResetGun();
                        break;

                    case ItemType.H_Parasite:
                        Debug.Log("PARASITE HAND DOESNT WORK");
                        Guns.instance.ResetGun();
                        break;
                        #endregion
                }
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.RemoveModifier(_slot.item.buffs[i]);
                    }
                }

                break;
            default:
                break;
        }
    }

    public void OnAfterSlotUpdate(InventorySlot _slot)
    {
        if (_slot.ItemObject == null)
            return;
        switch (_slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                print(string.Concat("Placed ", _slot.ItemObject, " on ", _slot.parent.inventory.type, ", Allowed Items: ", string.Join(", ", _slot.AllowedItems)));
                SoundManager.instance.PlaySound(equipmentClip, transform, 1.5f, false);
                switch(_slot.ItemObject.type)
                {
                    #region Normal Organs
                    case ItemType.O_Heart:
                        Debug.Log("HEART WORKS");
                        PlayerHealth.instance._maxlife += 20;
                        FullscreenShader.instance.normalShaderEnabled = true;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        break;

                    case ItemType.O_Liver:
                        Debug.Log("LIVER WORKS");
                        FlyweightPointer.Player.Damage += 5;
                        FullscreenShader.instance.normalShaderEnabled = true;
                        HotbarPlayer.Instance.AddToHotbar(ItemType.O_Liver);
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        break;

                    case ItemType.O_Lungs:
                        Debug.Log("LUNGS WORKS");
                        PlayerMovementAdvanced.instance.sprintSpeed += 2;
                        FullscreenShader.instance.normalShaderEnabled = true;
                        HotbarPlayer.Instance.AddToHotbar(ItemType.O_Lungs);
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        break;

                    case ItemType.O_Stomach:
                        Debug.Log("STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += 3;
                        FullscreenShader.instance.normalShaderEnabled = true;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        HotbarPlayer.Instance.AddToHotbar(ItemType.O_Stomach);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        break;
                    #endregion

                    #region Blazing Organs
                    case ItemType.O_BlazingHeart:
                        Debug.Log("BLAZING HEART WORKS");
                        PlayerHealth.instance._maxlife += 10;
                        FullscreenShader.instance.blazingShaderEnabled = true;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_BlazingLiver:
                        Debug.Log("BLAZING LIVER WORKS");
                        FlyweightPointer.Player.Damage += 10;
                        FullscreenShader.instance.blazingShaderEnabled = true;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_BlazingLungs:
                        Debug.Log("BLAZING LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 3;
                        FullscreenShader.instance.blazingShaderEnabled = true;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_BlazingStomach:
                        Debug.Log("BLAZING STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += 1;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        FullscreenShader.instance.blazingShaderEnabled = true;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        break;
                    #endregion

                    #region Cursed Organs
                    case ItemType.O_CursedHeart:
                        Debug.Log("CURSED HEART WORKS");
                        PlayerHealth.instance._maxlife -= 10;
                        PlayerHealth.instance.reviveTime += 3;
                        FullscreenShader.instance.cursedShaderEnabled = true;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_CursedLiver:
                        Debug.Log("CURSED LIVER WORKS");
                        PlayerHealth.instance._maxlife -= 10;
                        FlyweightPointer.Player.Damage += 20;
                        FullscreenShader.instance.cursedShaderEnabled = true;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_CursedLungs:
                        Debug.Log("CURSED LUNGS WORK");
                        PlayerHealth.instance._maxlife -= 10;
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 1;
                        PlayerMovementAdvanced.instance.climbSpeed += 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 2;
                        FullscreenShader.instance.cursedShaderEnabled = true;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_CursedStomach:
                        Debug.Log("CURSED STOMACH WORKS");
                        PlayerHealth.instance._maxlife -= 10;
                        PlayerMovementAdvanced.instance.jumpForce += 3;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        FullscreenShader.instance.cursedShaderEnabled = true;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        break;
                    #endregion

                    #region Vengeful Organs
                    case ItemType.O_VengefulHeart:
                        Debug.Log("VENGEFUL HEART WORK");
                        PlayerHealth.instance._maxlife += 15;
                        PlayerHealth.instance.lifeSteal += 2;
                        FullscreenShader.instance.vengefulShaderEnabled = true;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_VengefulLiver:
                        Debug.Log("VENGEFUL LIVER WORK");
                        PlayerHealth.instance.lifeSteal += 2;
                        FlyweightPointer.Player.Damage += 10;
                        FullscreenShader.instance.vengefulShaderEnabled = true;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_VengefulLungs:
                        Debug.Log("VENGEFUL LUNGS WORK");
                        PlayerHealth.instance.lifeSteal += 2;
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 1;
                        PlayerMovementAdvanced.instance.climbSpeed += 1;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 1;
                        FullscreenShader.instance.vengefulShaderEnabled = true;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_VengefulStomach:
                        Debug.Log("VENGEFUL STOMACH WORK");
                        PlayerHealth.instance.lifeSteal += 2;
                        PlayerMovementAdvanced.instance.jumpForce += 1;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        FullscreenShader.instance.vengefulShaderEnabled = true;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;
                    #endregion

                    #region Blessed Organs
                    case ItemType.O_BlessedHeart:
                        Debug.Log("BLESSED HEART WORK");
                        /*PlayerHealth.instance._maxlife += 25;
                        PlayerHealth.instance.shieldAmount += 25;
                        PlayerHealth.instance.shieldMax += 25;*/
                        FullscreenShader.instance.blessedShaderEnabled = true;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blessedOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_BlessedLiver:
                        Debug.Log("BLESSED LIVER WORK");
                       /* PlayerHealth.instance.shieldAmount += 25;
                        PlayerHealth.instance.shieldMax += 25;*/
                        FlyweightPointer.Player.Damage += 10;
                        FullscreenShader.instance.blessedShaderEnabled = true;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blessedOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_BlessedLungs:
                        Debug.Log("BLESSED LUNGS WORK");
                       /* PlayerHealth.instance.shieldAmount += 25;
                        PlayerHealth.instance.shieldMax += 25;*/
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 1;
                        PlayerMovementAdvanced.instance.climbSpeed += 1;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 1;
                        FullscreenShader.instance.blessedShaderEnabled = true;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blessedOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_BlessedStomach:
                        Debug.Log("BLESSED STOMACH WORK");
                       /* PlayerHealth.instance.shieldAmount += 25;
                        PlayerHealth.instance.shieldMax += 25;*/
                        PlayerMovementAdvanced.instance.jumpForce += 1;
                        PlayerMovementAdvanced.instance.airMultiplier += 2;
                        FullscreenShader.instance.blessedShaderEnabled = true;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blessedOrganClip, transform, 1f, false);
                        break;
                    #endregion

                    #region Forgotten Organs

                    case ItemType.O_ForgottenHeart:
                        Debug.Log("FORGOTTEN HEART WORKS");
                        PlayerHealth.instance._maxlife += forgottenHeartStat + forgottenBonus;
                        forgottenBonus += 5;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_ForgottenLungs:
                        Debug.Log("FORGOTTEN LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed += forgottenLungsStat + forgottenBonus;
                        PlayerMovementAdvanced.instance.slideSpeed += forgottenLungsStat + forgottenBonus;
                        PlayerMovementAdvanced.instance.climbSpeed += forgottenLungsStat + forgottenBonus;
                        PlayerMovementAdvanced.instance.wallrunSpeed += forgottenLungsStat + forgottenBonus;
                        forgottenBonus += 5;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_ForgottenLiver:
                        Debug.Log("FORGOTTEN LIVER WORKS");
                        FlyweightPointer.Player.Damage += forgottenLiverStat + forgottenBonus;
                        forgottenBonus += 5;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_ForgottenStomach:
                        Debug.Log("FORGOTTEN STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += forgottenStomachStat + forgottenBonus;
                        PlayerMovementAdvanced.instance.airMultiplier += forgottenStomachStat + forgottenBonus;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    #endregion

                    #region Spectral Organs

                    case ItemType.O_SpectralHeart:
                        Debug.Log("SPECTRAL HEART WORKS");
                        PlayerHealth.instance._maxlife += 15;
                        PlayerHealth.instance.dodgeChance += 14;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_SpectralLungs:
                        Debug.Log("SPECTRAL LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed += 2;
                        PlayerMovementAdvanced.instance.slideSpeed += 2;
                        PlayerMovementAdvanced.instance.climbSpeed += 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 2;
                        PlayerHealth.instance.dodgeChance += 7;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_SpectralLiver:
                        Debug.Log("SPECTRAL LIVER WORKS");
                        FlyweightPointer.Player.Damage += 15;
                        PlayerHealth.instance.dodgeChance += 7;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_SpectralStomach:
                        Debug.Log("SPECTRAL STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += 1;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        PlayerHealth.instance.dodgeChance += 7;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    #endregion

                    #region Lightning Organs

                    case ItemType.O_LightningHeart:
                        Debug.Log("LIGHTNING HEART WORKS");
                        PlayerHealth.instance._maxlife += 35;
                        Guns.instance.lightningDamage += 5;
                        Guns.instance.lightningChance += 7;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_LightningLungs:
                        Debug.Log("LIGHTNING LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed += 3;
                        PlayerMovementAdvanced.instance.slideSpeed += 3;
                        PlayerMovementAdvanced.instance.climbSpeed += 1;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 1;
                        Guns.instance.lightningDamage += 5;
                        Guns.instance.lightningChance += 7;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_LightningLiver:
                        Debug.Log("LIGHTNING LIVER WORKS");
                        FlyweightPointer.Player.Damage += 15;
                        Guns.instance.lightningDamage += 5;
                        Guns.instance.lightningChance += 7;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_LightningStomach:
                        Debug.Log("LIGHTING STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += 1;
                        PlayerMovementAdvanced.instance.airMultiplier += 2;
                        Guns.instance.lightningDamage += 5;
                        Guns.instance.lightningChance += 7;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    #endregion

                    #region Infernal Organs

                    case ItemType.O_InfernalHeart:
                        Debug.Log("INFERNAL HEART WORKS");
                        PlayerHealth.instance._maxlife += 10;
                        Guns.instance.infernalDamage += 5;
                        Guns.instance.infernalChance += 7;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_InfernalLungs:
                        Debug.Log("INFERNAL LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 1;
                        PlayerMovementAdvanced.instance.climbSpeed += 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 2;
                        Guns.instance.infernalDamage += 5;
                        Guns.instance.infernalChance += 7;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_InfernalLiver:
                        Debug.Log("INFERNAL LIVER WORKS");
                        FlyweightPointer.Player.Damage += 25;
                        Guns.instance.infernalDamage += 5;
                        Guns.instance.infernalChance += 7;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_InfernalStomach:
                        Debug.Log("INFERNAL STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += 2;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        Guns.instance.infernalDamage += 5;
                        Guns.instance.infernalChance += 7;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    #endregion

                    #region Rotten Organs

                    case ItemType.O_RottenHeart:
                        Debug.Log("ROTTEN HEART WORKS");
                        PlayerHealth.instance._maxlife += 10;
                        Guns.instance.rottenDamage += 5;
                        Guns.instance.rottenChance += 7;
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_RottenLungs:
                        Debug.Log("ROTTEN LUNGS WORK");
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 1;
                        PlayerMovementAdvanced.instance.climbSpeed += 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 2;
                        Guns.instance.rottenDamage += 5;
                        Guns.instance.rottenChance += 7;
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_RottenLiver:
                        Debug.Log("ROTTEN LIVER WORKS");
                        FlyweightPointer.Player.Damage += 25;
                        Guns.instance.rottenDamage += 5;
                        Guns.instance.rottenChance += 7;
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    case ItemType.O_RottenStomach:
                        Debug.Log("ROTTEN STOMACH WORKS");
                        PlayerMovementAdvanced.instance.jumpForce += 2;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        Guns.instance.rottenDamage += 5;
                        Guns.instance.rottenChance += 7;
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(vengefulOrganClip, transform, 1f, false);
                        break;

                    #endregion

                    #region Hands
                    case ItemType.H_Skeleton:
                        Debug.Log("SKELETON HAND WORKS");
                        Guns.instance.ResetGun();
                        Guns.instance.SkeletonHand();
                        break;

                    case ItemType.H_Knuckle:
                        Debug.Log("Knuckle Buster WORKS");
                        Guns.instance.ResetGun();
                        Guns.instance.KnuckleBuster();
                        break;

                    case ItemType.H_Invoker:
                        Debug.Log("Invoker HAND WORKS");
                        Guns.instance.ResetGun();
                        Guns.instance.InvokerHand();
                        break;

                    case ItemType.H_Teeth:
                        Debug.Log("TEETH SHOT WORKS");
                        Guns.instance.ResetGun();
                        Guns.instance.TeethShot();
                        break;

                    case ItemType.H_Nail:
                        Debug.Log("NAIL HAND WORKS");
                        Guns.instance.ResetGun();
                        Guns.instance.NailHand();  
                        break;

                    case ItemType.H_Parasite:
                        Debug.Log("PARASITE HAND WORKS");
                        Guns.instance.ResetGun();
                        Guns.instance.ParasiteHand();
                        break;
                        #endregion
                }
                for (int i = 0; i < _slot.item.buffs.Length; i++)
                {
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        if (attributes[j].type == _slot.item.buffs[i].attribute)
                            attributes[j].value.AddModifier(_slot.item.buffs[i]);
                    }
                }

                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var groundItem = other.GetComponent<GroundItem>();
        if (groundItem)
        {
            Item _item = new Item(groundItem.item);
            if (inventory.AddItem(_item, 1)) 
            {
                SoundManager.instance.PlaySound(groundPickUpClip, transform, 1.5f, false);
                Destroy(other.gameObject);
            }
        }
    }

    public void AttributeModified(Attribute attribute)
    {
        Debug.Log(string.Concat(attribute.type, " was updated! Value is now ", attribute.value.ModifiedValue));
    }


    private void OnApplicationQuit()
    {
        inventory.Clear();
        equipment.Clear();
    }
}

[System.Serializable]
public class Attribute
{
    [System.NonSerialized]
    public PlayerStats parent;
    public Attributes type;
    public ModifiableInt value;
    
    public void SetParent(PlayerStats _parent)
    {
        parent = _parent;
        value = new ModifiableInt(AttributeModified);
    }
    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}