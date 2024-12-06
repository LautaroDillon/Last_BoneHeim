using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiControl : MonoBehaviour
{
    [Header("References")]
    public Canvas pauseMenu;
    public Canvas invMenu;
    public Canvas tooltipMenu;

    [Header("Bools")]
    public static bool _isPaused;
    public static bool _isInventory;

    [Header("Sounds")]
    [SerializeField] private AudioClip menuSoundClip;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

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
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f, false);
            }
            else
            {
                InventoryOn();
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f, false);
            }
        }
    }

    void InventoryOn()
    {
        invMenu.gameObject.SetActive(true);
        tooltipMenu.gameObject.SetActive(true);
        PauseSlow();
        GameManager.instance.isRunning = false;
        _isInventory = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void InventoryOff()
    {
        invMenu.gameObject.SetActive(false);
        tooltipMenu.gameObject.SetActive(false);
        GameManager.instance.isRunning = true;
        Time.timeScale = 1.0f;
        _isInventory = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void TogglePause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            invMenu.gameObject.SetActive(false);
            tooltipMenu.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (_isPaused)
            {
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f, false);
                PauseOff();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SoundManager.instance.PlaySound(menuSoundClip, transform, 0.5f, false);
                PauseOn();
            }
        }
    }

    public void PauseOn()
    {
        _isPaused = true;
        GameManager.instance.isRunning = false;
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PauseSlow()
    {
        Time.timeScale = 0.10f;
        GameManager.instance.isRunning = false;
    }

    public void PauseOff()
    {
        _isPaused = false;
        pauseMenu.gameObject.SetActive(false);
        GameManager.instance.isRunning = true;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
