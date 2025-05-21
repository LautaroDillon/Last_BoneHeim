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

    [InspectorName("Grups Enemys ")]
    public Dictionary<int, List<E_Shooter>> _groups = new Dictionary<int, List<E_Shooter>>();


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
    //El peso que va a tener cada metodo. Cual quiero que sea mas prioritario
    public float weightSeparation = 4f; 
    [Range(0, 5f)]
    public float weightAlingment = 3f;

    public bool InLineOfSight(Vector3 start, Vector3 end)//lo tengo a simple vista? 
    {
        var dir = end - start;
        //Si no hay ningun objeto de con la layer "maskWall" entonces
        //quiere decir que estoy viendo a mi objetico (por eso lo invierto para que me de True)
        return !Physics.Raycast(start, dir, dir.magnitude, maskWall); 
    }

    public void RegisterShooter(E_Shooter shooter)
    {
        if (!_groups.TryGetValue(shooter.zoneId, out var list))
        {
            list = new List<E_Shooter>();
            _groups[shooter.zoneId] = list;
        }
        shooter.groupIndex = list.Count;      // le damos su posici�n en la formaci�n
        list.Add(shooter);
    }

    public void oneseeplayer(int id)
    {
        if (_groups.TryGetValue(id, out List<E_Shooter> shooters))
        {
            foreach (var shooter in shooters)
            {
                shooter.canSeePlayer = true; // o cualquier otro bool
                shooter.otherSeenPlayer = true; // o cualquier otro bool
                StartCoroutine(shooter.longtime(8));
            }
        }
    }

}
