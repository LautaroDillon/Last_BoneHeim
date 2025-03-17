using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] GameObject tutorial;
    public bool invTutorialDone;

    private void Start()
    {
        tutorial.gameObject.SetActive(false);
        invTutorialDone = false;
    }

    private void Update()
    {
        if (tutorial.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                TutorialEnd();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            tutorial.gameObject.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameManager.instance.isRunning = false;
        }
    }

    public void TutorialEnd()
    {
        tutorial.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        GameManager.instance.isRunning = true;
        Destroy(gameObject);
    }
}
