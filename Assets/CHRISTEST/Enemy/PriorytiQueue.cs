using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priority<T>
{
    private class Entry
    {
        public T Element;
        public float Priority;
        public int InsertionIndex;

        public Entry(T element, float priority, int insertionIndex)
        {
            Element = element;
            Priority = priority;
            InsertionIndex = insertionIndex;
        }
    }

    private int _insertionCounter = 0;
    private readonly Dictionary<T, Entry> _lookup = new();
    private readonly SortedSet<Entry> _sortedSet;

    public int Count => _lookup.Count;

    public Priority()
    {
        _sortedSet = new SortedSet<Entry>(new EntryComparer());
    }

    public void Enqueue(T elem, float cost)
    {
        if (_lookup.TryGetValue(elem, out var existing))
        {
            _sortedSet.Remove(existing);
        }

        var entry = new Entry(elem, cost, _insertionCounter++);
        _lookup[elem] = entry;
        _sortedSet.Add(entry);
    }

    public T Dequeue()
    {
        if (_sortedSet.Count == 0) return default;

        var first = _sortedSet.Min;
        _sortedSet.Remove(first);
        _lookup.Remove(first.Element);
        return first.Element;
    }

    public bool ContainsKey(T elem) => _lookup.ContainsKey(elem);

    public float GetPriority(T elem)
    {
        return _lookup.TryGetValue(elem, out var entry) ? entry.Priority : Mathf.Infinity;
    }

    public void UpdatePriority(T elem, float newCost)
    {
        if (_lookup.TryGetValue(elem, out var existing))
        {
            _sortedSet.Remove(existing);
            var updated = new Entry(elem, newCost, existing.InsertionIndex);
            _lookup[elem] = updated;
            _sortedSet.Add(updated);
        }
    }

    public void Clear()
    {
        _lookup.Clear();
        _sortedSet.Clear();
        _insertionCounter = 0;
    }

    private class EntryComparer : IComparer<Entry>
    {
        public int Compare(Entry a, Entry b)
        {
            int cmp = a.Priority.CompareTo(b.Priority);
            return cmp != 0 ? cmp : a.InsertionIndex.CompareTo(b.InsertionIndex);
        }
    }
}
