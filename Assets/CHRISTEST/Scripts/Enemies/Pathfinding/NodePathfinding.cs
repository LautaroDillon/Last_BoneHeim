using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePathfinding : MonoBehaviour
{
    public List<NodePathfinding> neighbors = new List<NodePathfinding>();
    public int cost;

    [Header("Zona")]
    public int zoneId;// <— identifica a qué zona pertenece este nodo



    private void OnDrawGizmos()
    {
        Gizmos.color = (zoneId == 0 ? Color.white : Color.Lerp(Color.red, Color.green, zoneId / 5f));
        Gizmos.DrawSphere(transform.position, 0.2f);
        foreach (var n in neighbors)
            Gizmos.DrawLine(transform.position, n.transform.position);
    }
}

