using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    public void LoadScene(int level)
    {

        SceneManager.LoadScene(level);
        inventory.Clear();
        equipment.Clear();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
