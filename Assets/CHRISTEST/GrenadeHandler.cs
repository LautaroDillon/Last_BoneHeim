using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct GrenadePrefabMapping
{
    public GrenadeType type;
    public GameObject prefab;
}

public class GrenadeHandler : MonoBehaviour
{
    public static GrenadeHandler Instance;

    [SerializeField] private List<GrenadePrefabMapping> grenadePrefabs;

    private Dictionary<GrenadeType, GameObject> prefabDict;

    private void Awake()
    {
        Instance = this;
        prefabDict = grenadePrefabs.ToDictionary(x => x.type, x => x.prefab);
    }

    public GameObject GetGrenadePrefab(GrenadeType type)
    {
        if (!prefabDict.TryGetValue(type, out var prefab))
        {
            Debug.LogError($"Grenade prefab not found for type: {type}");
            return null;
        }

        return prefab;
    }
}
