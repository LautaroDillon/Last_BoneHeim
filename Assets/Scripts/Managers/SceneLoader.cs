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
        FullscreenShader.instance.acidShaderEnabled = false;
        FullscreenShader.instance.armShaderEnabled = false;
        FullscreenShader.instance.blazingShaderEnabled = false;
        FullscreenShader.instance.cursedShaderEnabled = false;
        FullscreenShader.instance.normalShaderEnabled = false;
        FullscreenShader.instance.speedShaderEnabled = false;
        FullscreenShader.instance.vengefulShaderEnabled = false;
        FullscreenShader.instance.blessedShaderEnabled = false;
        SceneManager.LoadScene(level);
        inventory.Clear();
        equipment.Clear();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
