using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public KeyCode Pause = KeyCode.Escape;

    [Header("Assign Player")]
    public Transform thisIsPlayer;
    public GameObject player;
    public List<GameObject> enemies;

    [Header("Time Scale Check")]
    public bool isRunning;

    private void Update()
    {
        
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
        enemies.Add(t);
    }

}
