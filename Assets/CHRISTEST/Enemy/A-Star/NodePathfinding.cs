using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NodePathfinding : MonoBehaviour
{
    public List<NodePathfinding> neighbors = new List<NodePathfinding>();
    public int cost;

    [Header("Zone Info")]
    public int zoneId;

    private void OnDrawGizmos()
    {
        // Color node based on zoneId
        Gizmos.color = GetZoneColor(zoneId);
        Gizmos.DrawSphere(transform.position, 0.2f);

        // Draw neighbor lines
        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null)
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }

#if UNITY_EDITOR
        // Draw zone ID label in scene view
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.25f, $"Zone {zoneId}");
#endif
    }

    private Color GetZoneColor(int zone)
    {
        if (zone <= 0) return Color.white;

        // Cycle colors based on zone ID
        return Color.HSVToRGB((zone * 0.1f) % 1f, 0.8f, 1f);
    }
}

