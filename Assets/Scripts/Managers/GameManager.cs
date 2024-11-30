using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Assign Player")]
    public Transform thisIsPlayer;
    public List<GameObject> enemys;

    [Header("Time Scale Check")]
    public bool isRunning;
    void Start()
    {
        instance = this;
        isRunning = true;
    }

    public void AddToList(GameObject t)
    {
        enemys.Add(t);
    }

}
