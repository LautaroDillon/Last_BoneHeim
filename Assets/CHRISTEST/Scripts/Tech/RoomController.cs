using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject door;
    public Entity[] enemies;

    private bool doorOpened = false;

    void Update()
    {
        if (!doorOpened && AllEnemiesDefeated())
        {
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

        doorOpened = true;
        Debug.Log("All enemies defeated! Door opened.");
    }
}
