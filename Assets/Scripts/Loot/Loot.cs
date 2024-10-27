using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Loot : ScriptableObject 
{   
    public GameObject lootObject;
    public string lootName;
    public int dropChance;

    public Loot(string _LootName , int _DropChance)
    {
        this.lootName = _LootName;
        this.dropChance = _DropChance;
    }

}
public struct LootInfo
{
    public string nombre;
    public float chanceDeLooteo;

    public LootInfo(Loot lootData)
    {
        nombre = lootData.lootName;
        chanceDeLooteo = lootData.dropChance;
    }
}
