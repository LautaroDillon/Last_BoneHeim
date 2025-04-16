using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public KeyCode changeScene = KeyCode.Alpha1;
    public KeyCode changeScene2 = KeyCode.Alpha2;
    public KeyCode close = KeyCode.Escape;


    [Header("Assign Player")]
    public Transform thisIsPlayer;
    public List<GameObject> enemys;

    [Header("Time Scale Check")]
    public bool isRunning;

    private void Update()
    {
        if (Input.GetKeyDown(changeScene))
        {
            SceneManager.LoadScene(0);

        }  
        if (Input.GetKeyDown(changeScene2))
        {
            SceneManager.LoadScene(1);

        }
        if (Input.GetKeyDown(close))
        {
            Application.Quit();

        }
    }

    private void Awake()
    {
        isRunning = true;
    }
    void Start()
    {
        instance = this;
    }
    public void AddToList(GameObject t)
    {
        enemys.Add(t);
    }

}
