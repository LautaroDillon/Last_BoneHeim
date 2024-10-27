using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiControl : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas invMenu;
    public Canvas tooltipMenu;
    public static bool _isPaused;
    public static bool _isInventory;
    [SerializeField] private AudioClip menuSoundClip;

    private void Start()
    {
        pauseMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        TogglePause();
        Inventory();
    }

    void Inventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_isInventory)
            {
                InventoryOff();
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f);
            }
            else
            {
                InventoryOn();
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f);
            }
        }
    }

    void InventoryOn()
    {
        invMenu.gameObject.SetActive(true);
        tooltipMenu.gameObject.SetActive(true);
        PauseSlow();
        _isInventory = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void InventoryOff()
    {
        invMenu.gameObject.SetActive(false);
        tooltipMenu.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        _isInventory = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void TogglePause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f);
                PauseOff();
            }
            else
            {
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f);
                PauseOn();
            }
        }
    }

    public void PauseOn()
    {
        _isPaused = true;
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PauseSlow()
    {
        Time.timeScale = 0.10f;
    }

    public void PauseOff()
    {
        _isPaused = false;
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
