using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public KeyCode Pause = KeyCode.Escape;
    public GameObject pauseMenu;
    public static bool isPaused;
    public ControlDOF cdof;
    public List<GameObject> otherPages;

    private void Awake()
    {
        pauseMenu.SetActive(false);

        foreach (GameObject page in otherPages)
        {
            page.SetActive(true);
        }
    }

    private void Update()
    {
        if (DeathUI.deathUiActive)
            return;
        else
        {
            if (Input.GetKeyDown(Pause))
            {
                if (isPaused)
                {
                    UnPauseGame();
                    AudioManager.instance.UnPauseSFX();
                }
                else
                {
                    PauseGame();
                    AudioManager.instance.PauseSFX();
                }
            }
        }
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (GameObject page in otherPages)
        {
            page.SetActive(false);
        }

        AudioManager.instance.PauseMenuMusic();

        if (cdof != null)
        {
            cdof.EnablePauseDOF();
            cdof.LerpFocalLength(cdof.normalFocalLength, cdof.pauseFocalLength, 0.2f);
        }
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (GameObject page in otherPages)
        {
            page.SetActive(true);
        }

        AudioManager.instance.UnPauseMenuMusic();

        if(cdof != null)
        {
            cdof.DisablePauseDOF();
            cdof.LerpFocalLength(cdof.pauseFocalLength, cdof.normalFocalLength, 0.2f);
        }
    }
}
