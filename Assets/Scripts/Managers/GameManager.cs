using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Transform thisIsPlayer;

    public List<GameObject> enemys;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void AddToList(GameObject t)
    {
        enemys.Add(t);
    }

}
