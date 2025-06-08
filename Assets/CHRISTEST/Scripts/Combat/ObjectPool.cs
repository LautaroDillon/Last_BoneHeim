using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int initialSize;
    }

    public List<PoolItem> items;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var item in items)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < item.initialSize; i++)
            {
                var obj = Instantiate(item.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDictionary[item.prefab] = queue;
        }
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("Pool for prefab not found");
            return null;
        }

        var queue = poolDictionary[prefab];
        GameObject obj = queue.Count > 0 ? queue.Dequeue() : Instantiate(prefab);

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        foreach (var kvp in poolDictionary)
        {
            if (obj.name.StartsWith(kvp.Key.name)) // crude check; you can use a wrapper class instead
            {
                kvp.Value.Enqueue(obj);
                return;
            }
        }

        Destroy(obj); // fallback if not from pool
    }
}
