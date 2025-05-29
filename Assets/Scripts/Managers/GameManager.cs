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
    public List<GameObject> enemies = new List<GameObject>();

    [Header("Time Scale Check")]
    public bool isRunning;

    public LayerMask maskWall;

    [InspectorName("Groups Enemies")]
    public Dictionary<int, List<E_Shooter>> _groups = new Dictionary<int, List<E_Shooter>>();

    [Range(0, 5f)]
    public float weightSeparation = 4f;

    [Range(0, 5f)]
    public float weightAlingment = 3f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        isRunning = true;
    }

    public void AddToList(GameObject t)
    {
        if (!enemies.Contains(t))
            enemies.Add(t);
    }

    public bool InLineOfSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        // Returns true if no obstacle between start and end
        return !Physics.Raycast(start, dir, dir.magnitude, maskWall);
    }

    public void RegisterShooter(E_Shooter shooter)
    {
        if (!_groups.TryGetValue(shooter.zoneId, out var list))
        {
            list = new List<E_Shooter>();
            _groups[shooter.zoneId] = list;
        }
        shooter.groupIndex = list.Count; // Assign index in the group
        list.Add(shooter);
    }

    public void RemoveShooter(E_Shooter shooter)
    {
        if (_groups.TryGetValue(shooter.zoneId, out var list))
        {
            list.Remove(shooter);
            // Optionally, update groupIndex for remaining shooters
            for (int i = 0; i < list.Count; i++)
            {
                list[i].groupIndex = i;
            }
        }
    }

    public void OnOneSeePlayer(int id)
    {
        if (_groups.TryGetValue(id, out List<E_Shooter> shooters))
        {
            foreach (var shooter in shooters)
            {
                shooter.canSeePlayer = true;
                shooter.otherSeenPlayer = true;
            }
        }
    }

    public bool RandomChance(int min, int max, int threshold)
    {
        int result = Random.Range(min, max);
        return result <= threshold;
    }

    private void Update()
    {
        // Pause toggle logic example
        if (Input.GetKeyDown(Pause))
        {
            isRunning = !isRunning;
            Time.timeScale = isRunning ? 1f : 0f;
        }
    }
}
