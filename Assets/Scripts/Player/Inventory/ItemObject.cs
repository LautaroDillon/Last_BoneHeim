using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{ //Agregar desde abajo de todo, porque agregan desde, por ejemplo, O_Lungs, corre toda la lista y todos los items se van a mover la cantidad de espacios que agregaron.
  // El default no importa porque no se usa

    //Organos comunes, mejoras de stats directos
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

    //Resta vida a cambio de stats mejores
    O_CursedHeart,
    O_CursedLungs,
    O_CursedStomach,
    O_CursedLiver,

    //Stats mejores que los comunes
    O_BlazingHeart,
    O_BlazingLungs,
    O_BlazingStomach,
    O_BlazingLiver,

    //Ademas de mejorar stats, agrega robo de vida
    O_VengefulHeart,
    O_VengefulLungs,
    O_VengefulStomach,
    O_VengefulLiver,

    //Ademas de mejorar stats, otorga un escudo al jugador
    O_BlessedHeart,
    O_BlessedLungs,
    O_BlessedStomach,
    O_BlessedLiver,

    H_Nail,
    H_Parasite,

    //Mejora stats, agrega una chance que, al golpear un enemigo, genere un rayo que ataque un enemigo cercano, cada organo mejora su chance, y si tenes la mano electrica, mejora su daño
    O_LightningHeart,
    O_LightningLungs,
    O_LightningLiver,
    O_LightningStomach,

    //Mejora stats, al deslizarte, generas una linea de fuego, cada organo aumenta su daño, si tenes la mano infernal, mejora su daño
    O_InfernalHeart,
    O_InfernalLungs,
    O_InfernalLiver,
    O_InfernalStomach,

    //Mejora stats de forma aleatora en un rango determinado, cada organo equipado mejora sus chances de tener buenos stats
    O_ForgottenHeart,
    O_ForgottenLungs,
    O_ForgottenLiver,
    O_ForgottenStomach,

    //Mejora stats, da una chance de evitar daño
    O_SpectralHeart,
    O_SpectralLungs,
    O_SpectralLiver,
    O_SpectralStomach,

    //Mejora stats, aplica veneno a enemigos, mas organos = mas chance. Si tenes la Rotten Hand, mejora el daño del veneno
    O_RottenHeart,
    O_RottenLungs,
    O_RottenLiver,
    O_RottenStomach,

    //Mejora stats, aplica un empuje cuando una bala impacta un enemigo, si choca contra una pared le hara daño, mas organos = fuerza del empuje y daño. Enemigos muertos por el impacto explotan.
    O_HatefulHeart,
    O_HatefulLungs,
    O_HatefulLiver,
    O_HatefulStomach,

    H_Electric,  //Dispara proyectiles que tienen una chance de electrocutar enemigos cercanos, estuneandolos y dañandolos
    H_Rotten,  //Dispara proyectiles que pueden envenenar enemigos, causandoles daño
    H_Infernal, //Lanza llamas, prendiendo los enemigos en fuego
    H_Spawn, //Dispara proyectiles normales, use el fuego alternativo para sacrificar vida para spawnea criaturas que te ayudan en combate
    H_Abysmal, //Dispara proyectiles normales, use el fuego alternativo para generar un agujero negro que atrae enemigos
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