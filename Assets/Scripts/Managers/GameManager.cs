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

    public LayerMask maskWall;


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

    [Range(0, 5f)]
    public float weightSeparation = 4f; //El peso que va a tener cada metodo. Cual quiero que sea mas prioritario
    [Range(0, 5f)]
    public float weightAlingment = 3f;

    public bool InLineOfSight(Vector3 start, Vector3 end)//lo tengo a simple vista? 
    {
        var dir = end - start;

        return !Physics.Raycast(start, dir, dir.magnitude, maskWall); //Si no hay ningun objeto de con la layer "maskWall" entonces quiere decir que estoy viendo a mi objetico (por eso lo invierto para que me de True)
    }

}
