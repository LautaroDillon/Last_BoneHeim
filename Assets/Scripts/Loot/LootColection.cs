using UnityEngine;

public class LootColection : MonoBehaviour
{
    public Loot loot;

    public void ProcesarLoot()
    {
        LootInfo lootInfo = new LootInfo(loot);

        //Utilizar la información del lootInfo
        Debug.Log("Nombre del loot: " + lootInfo.nombre + " Chance de looteo: " + lootInfo.chanceDeLooteo);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ProcesarLoot();
            Destroy(this.gameObject);
        }
    }
}
