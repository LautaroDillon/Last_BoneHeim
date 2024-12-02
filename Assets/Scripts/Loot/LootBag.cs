using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public GameObject DroppeItemPrefab;
    public List<Loot> lootList = new List<Loot>();
    bool candrop;

    Loot GetDroppedLoot()
    {
        int randomNumber = Random.Range(1, 101);

        List<Loot> posibleItems = new List<Loot>();

        foreach (Loot item in lootList)
        {
            if (randomNumber <= item.dropChance)
            {
                posibleItems.Add(item);
            }
        }

        if (posibleItems.Count > 0)
        {
            Loot droppedItem = posibleItems[Random.Range(0, posibleItems.Count)];
            return droppedItem;
        }

        print("no Item");
        return null;
    }

    private void Start()
    {
        GetDroppedLoot();
    }

    public void intanceLoot(Vector3 SpawnPosition)
    {
        Loot droppedItem = GetDroppedLoot();
        Debug.Log("item que dropea"+ droppedItem.name);
        if (droppedItem != null && !candrop)
        {
            candrop = true;
            DroppeItemPrefab = droppedItem.lootObject;
            GameObject lootGameobject = Instantiate(DroppeItemPrefab, SpawnPosition, Quaternion.identity);
           // lootGameobject.GetComponent<Loot>().lootObject = droppedItem.lootObject;
            lootGameobject.GetComponent<LootColection>().loot = droppedItem;
        }
    }
}
