using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("References")]

    public InventoryObject inventory;
    public InventoryObject equipment;
    public Attribute[] attributes;

    [Header("Sounds")]

    [SerializeField] private AudioClip groundPickUpClip;
    [SerializeField] private AudioClip equipmentClip;
    [SerializeField] private AudioClip unequipClip;
    [SerializeField] protected AudioClip blazingOrganClip;
    [SerializeField] protected AudioClip cursedOrganClip;
    [SerializeField] protected AudioClip heartEquipClip;
    [SerializeField] protected AudioClip lungsEquipClip;
    [SerializeField] protected AudioClip liverEquipClip;
    [SerializeField] protected AudioClip stomachEquipClip;
    [SerializeField] protected AudioClip stomachSecondaryClip;

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
                SoundManager.instance.PlaySound(equipmentClip, transform, 1f, false);
                switch (_slot.ItemObject.type)
                {
                    #region Organs
                    case ItemType.O_Heart:
                        Debug.Log("HEART DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife -= 20;
                        break;

                    case ItemType.O_Liver:
                        Debug.Log("LIVER DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        FlyweightPointer.Player.Damage -= 5;
                        break;

                    case ItemType.O_Lungs:
                        Debug.Log("LUNGS DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.sprintSpeed -= 2;
                        break;

                    case ItemType.O_Stomach:
                        Debug.Log("STOMACH DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.jumpForce -= 3;
                        break;

                    case ItemType.O_BlazingHeart:
                        Debug.Log("BLAZING HEART DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife -= 10;
                        break;

                    case ItemType.O_BlazingLiver:
                        Debug.Log("BLAZING LIVER DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        FlyweightPointer.Player.Damage -= 10;
                        break;

                    case ItemType.O_BlazingLungs:
                        Debug.Log("BLAZING LUNGS DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 3;
                        break;

                    case ItemType.O_BlazingStomach:
                        Debug.Log("BLAZING STOMACH DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.jumpForce -= 1;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
                        break;

                    case ItemType.O_CursedHeart:
                        Debug.Log("CURSED HEART DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife += 10;
                        PlayerHealth.instance.reviveTime -= 3;
                        break;

                    case ItemType.O_CursedLiver:
                        Debug.Log("CURSED LIVER DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife += 10;
                        FlyweightPointer.Player.Damage -= 20;
                        break;

                    case ItemType.O_CursedLungs:
                        Debug.Log("CURSED LUNGS DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife += 10;
                        PlayerMovementAdvanced.instance.sprintSpeed -= 1;
                        PlayerMovementAdvanced.instance.slideSpeed -= 1;
                        PlayerMovementAdvanced.instance.climbSpeed -= 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed -= 2;
                        break;

                    case ItemType.O_CursedStomach:
                        Debug.Log("CURSED STOMACH DOESNT WORK");
                        SoundManager.instance.PlaySound(unequipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife += 10;
                        PlayerMovementAdvanced.instance.jumpForce -= 3;
                        PlayerMovementAdvanced.instance.airMultiplier -= 1;
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
                    #region Organs
                    case ItemType.O_Heart:
                        Debug.Log("HEART WORKS");
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife += 20;
                        break;

                    case ItemType.O_Liver:
                        Debug.Log("LIVER WORKS");
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        FlyweightPointer.Player.Damage += 5;
                        break;

                    case ItemType.O_Lungs:
                        Debug.Log("LUNGS WORKS");
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.sprintSpeed += 2;
                        break;

                    case ItemType.O_Stomach:
                        Debug.Log("STOMACH WORKS");
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.jumpForce += 3;
                        break;

                    case ItemType.O_BlazingHeart:
                        Debug.Log("BLAZING HEART WORKS");
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife += 10;
                        break;

                    case ItemType.O_BlazingLiver:
                        Debug.Log("BLAZING LIVER WORKS");
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        FlyweightPointer.Player.Damage += 10;
                        break;

                    case ItemType.O_BlazingLungs:
                        Debug.Log("BLAZING LUNGS WORK");
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 3;
                        break;

                    case ItemType.O_BlazingStomach:
                        Debug.Log("BLAZING STOMACH WORKS");
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(blazingOrganClip, transform, 1f, false);
                        PlayerMovementAdvanced.instance.jumpForce += 1;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        break;

                    case ItemType.O_CursedHeart:
                        Debug.Log("CURSED HEART WORKS");
                        SoundManager.instance.PlaySound(heartEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife -= 10;
                        PlayerHealth.instance.reviveTime += 3;
                        break;

                    case ItemType.O_CursedLiver:
                        Debug.Log("CURSED LIVER WORKS");
                        SoundManager.instance.PlaySound(liverEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife -= 10;
                        FlyweightPointer.Player.Damage += 20;
                        break;

                    case ItemType.O_CursedLungs:
                        Debug.Log("CURSED LUNGS WORK");
                        SoundManager.instance.PlaySound(lungsEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife -= 10;
                        PlayerMovementAdvanced.instance.sprintSpeed += 1;
                        PlayerMovementAdvanced.instance.slideSpeed += 1;
                        PlayerMovementAdvanced.instance.climbSpeed += 2;
                        PlayerMovementAdvanced.instance.wallrunSpeed += 2;
                        break;

                    case ItemType.O_CursedStomach:
                        Debug.Log("CURSED STOMACH WORKS");
                        SoundManager.instance.PlaySound(stomachEquipClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(stomachSecondaryClip, transform, 1f, false);
                        SoundManager.instance.PlaySound(cursedOrganClip, transform, 1f, false);
                        PlayerHealth.instance._maxlife -= 10;
                        PlayerMovementAdvanced.instance.jumpForce += 3;
                        PlayerMovementAdvanced.instance.airMultiplier += 1;
                        break;
                    #endregion

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
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.Save();
            equipment.Save();
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            inventory.Load();
            equipment.Load();
        }
        */
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