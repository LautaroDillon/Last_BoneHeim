﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{ //Agregar desde abajo de todo, porque agregan desde, por ejemplo, O_Lungs, corre toda la lista y todos los items se van a mover la cantidad de espacios que agregaron.
  // El default no importa porque no se usa
    O_Heart,
    O_Brain,
    O_Lungs,
    O_Stomach,
    O_Kidney,
    O_Liver,
    H_Skeleton,
    H_Invoker,
    H_Knuckle,
    H_Teeth,
    O_CursedHeart,
    O_CursedLungs,
    O_CursedStomach,
    O_CursedLiver,
    O_BlazingHeart,
    O_BlazingLungs,
    O_BlazingStomach,
    O_BlazingLiver,
    O_VengefulHeart,
    O_VengefulLungs,
    O_VengefulStomach,
    O_VengefulLiver,
    O_BlessedHeart,
    O_BlessedLungs,
    O_BlessedStomach,
    O_BlessedLiver,
    H_Nail,
    H_Parasite,
    Default
    
}

public enum Attributes
{
    [Header("Organ")]
    MoveSpeed,
    SlideSpeed,
    JumpForce,
    BoneRegen,
    [Header("Hand")]
    Damage,
    FireRate,
    Spread,
    TimeBetweenShots,
    BulletsPerShot,
    MagazineSize
}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObject : ScriptableObject
{

    public Sprite uiDisplay;
    public GameObject characterDisplay;
    public bool stackable;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    public Item data = new Item();
    
    public List<string> boneNames = new List<string>();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }


    private void OnValidate()
    {
        boneNames.Clear();
        if(characterDisplay == null)
            return;
        if(!characterDisplay.GetComponent<SkinnedMeshRenderer>())
            return;

        var renderer = characterDisplay.GetComponent<SkinnedMeshRenderer>();
        var bones = renderer.bones;

        foreach (var t in bones)
        {
            boneNames.Add(t.name);
        }

    }
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public Item()
    {
        Name = "";
        Id = -1;
    }
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
            {
                attribute = item.data.buffs[i].attribute
            };
        }
    }
}

[System.Serializable]
public class ItemBuff : IModifier
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}