using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : Entity
{
    private bool hasDied = false;

    private void Update()
    {
        Death();
    }

    private void Death()
    {
        if (isDead && !hasDied)
        {
            hasDied = true;
            AudioManager.instance.StopMusic("Background Music");
            Debug.Log("You dieded!");
            AudioManager.instance.PlayMusic("Death Music", 0.5f);
        }
    }
}
