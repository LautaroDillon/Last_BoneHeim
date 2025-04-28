using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject door;
    public GameObject player;
    public Entity[] enemies;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float forceStrength = 5f;

    private bool doorOpened = false;
    public bool lootDrop = false;

    void Update()
    {
        if (!doorOpened && AllEnemiesDefeated())
        {
            doorOpened = true;

            AudioManager.instance.PlaySFX("Door Unlock", 1f, false);

            if (lootDrop)
            {
                DropItem(player.transform.position + Vector3.up * 0.5f);
                AudioManager.instance.PlaySFXOneShot("Organ Drop", 1f);
            }
            OpenDoor();
        }
    }

    private bool AllEnemiesDefeated()
    {
        foreach (Entity enemy in enemies)
        {
            if (enemy != null && enemy.isDead == false)
            {
                return false;
            }
        }
        return true;
    }

    private void OpenDoor()
    {
        door.SetActive(false);
        Debug.Log("All enemies defeated! Door opened.");
    }

    public void DropItem(Vector3 dropPosition)
    {
        GameObject droppedItem = Instantiate(prefab, dropPosition, Quaternion.identity);
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

        Vector3 randomDirection = (Vector3.right * Random.Range(-1f, 1f)) + (Vector3.forward * Random.Range(-1f, 1f));
        Vector3 force = (randomDirection.normalized + Vector3.up) * forceStrength;

        rb.AddForce(force, ForceMode.Impulse);
    }
}
