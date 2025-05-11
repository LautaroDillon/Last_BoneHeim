using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prioryti<T>
{
    private Dictionary<T, float> _allElements = new();

    public int Count => _allElements.Count;

    public void Enqueue(T elem, float cost)
    {
        if (!_allElements.ContainsKey(elem))
            _allElements.Add(elem, cost);
        else
            _allElements[elem] = cost; // Opcional: sobrescribe si ya existe
    }

    public T Dequeue()
    {
        float lowestValue = Mathf.Infinity;
        T elem = default;

        foreach (var item in _allElements)
        {
            if (item.Value < lowestValue)
            {
                elem = item.Key;
                lowestValue = item.Value;
            }
        }

        _allElements.Remove(elem);
        return elem;
    }

    public bool ContainsKey(T elem) => _allElements.ContainsKey(elem);

    public float GetPriority(T elem)
    {
        if (_allElements.TryGetValue(elem, out float value))
            return value;
        return Mathf.Infinity;
    }

    public void UpdatePriority(T elem, float newCost)
    {
        if (_allElements.ContainsKey(elem))
            _allElements[elem] = newCost;
    }

    public void Clear() => _allElements.Clear();
}
