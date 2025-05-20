using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("References")]
    public ControlDOF cdof;
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Canvas")]
    public GameObject pauseMenu;
    public List<GameObject> hideWhenPause;

    private Stack<GameObject> pageStack = new Stack<GameObject>();
    public static bool isPaused = false;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        foreach (GameObject page in hideWhenPause)
        {
            page.SetActive(true);
        }
    }

    private void Update()
    {
        if (DeathUI.deathUiActive)
            return;

        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                if (pageStack.Count > 1)
                {
                    GoBackPage();
                }
                else
                {
                    UnPauseGame();
                }

                AudioManager.instance?.UnPauseSFX();
            }
            else
            {
                PauseGame();
                AudioManager.instance?.PauseSFX();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        isPaused = true;

        pageStack.Clear();
        pageStack.Push(pauseMenu);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (GameObject page in hideWhenPause)
        {
            page.SetActive(false);
        }

        AudioManager.instance?.PauseMenuMusic();

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

        foreach (GameObject page in hideWhenPause)
        {
            page.SetActive(true);
        }

        AudioManager.instance?.UnPauseMenuMusic();

        if (cdof != null)
        {
            cdof.DisablePauseDOF();
            cdof.LerpFocalLength(cdof.pauseFocalLength, cdof.normalFocalLength, 0.2f);
        }
    }

    void OpenPage(GameObject newPage)
    {
        if (pageStack.Count > 0)
        {
            GameObject currentPage = pageStack.Peek();
            FadeUI currentFader = currentPage.GetComponent<FadeUI>();
            if (currentFader != null)
                currentFader.FadeOut();
            else
                currentPage.SetActive(false);
        }

        FadeUI newFader = newPage.GetComponent<FadeUI>();
        if (newFader != null)
            newFader.FadeIn();
        else
            newPage.SetActive(true);

        pageStack.Push(newPage);
    }

    void GoBackPage()
    {
        if (pageStack.Count > 1)
        {
            GameObject topPage = pageStack.Pop();
            FadeUI topFader = topPage.GetComponent<FadeUI>();
            if (topFader != null)
                topFader.FadeOut();
            else
                topPage.SetActive(false);

            GameObject previousPage = pageStack.Peek();
            FadeUI prevFader = previousPage.GetComponent<FadeUI>();
            if (prevFader != null)
                prevFader.FadeIn();
            else
                previousPage.SetActive(true);
        }
        else
        {
            UnPauseGame();
        }
    }
}
