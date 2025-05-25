using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerNode : MonoBehaviour
{
    public static ManagerNode Instance;
    public List<NodePathfinding> nodes;

    [Header("Ajustes de conexión")]
    public float maxNeighborDistance = 3f;   // distancia máxima en planta
    public float maxStepHeight = 1.5f;

    private void Awake()
    {
        Instance = this;
        // Carga automática de todos los nodos en escena
        nodes = FindObjectsOfType<NodePathfinding>().ToList();
        Debug.Log($"[ManagerNode] {nodes.Count} nodos cargados");
    }

    /*private void Start()
    {
        foreach (var a in nodes)
        {
            int connected = 0;
            foreach (var b in nodes)
            {
                if (a == b) continue;

                // Distancia planar
                float dx = a.transform.position.x - b.transform.position.x;
                float dz = a.transform.position.z - b.transform.position.z;
                float planarDist = Mathf.Sqrt(dx * dx + dz * dz);

                // Diferencia de altura
                float heightDiff = Mathf.Abs(a.transform.position.y - b.transform.position.y);

                // Comprobación de línea de vista usando GameManager
                bool canSee = GameManager.instance.InLineOfSight(
                    a.transform.position + Vector3.up * 0.5f,
                    b.transform.position + Vector3.up * 0.5f
                );

                if (planarDist <= maxNeighborDistance
                 && heightDiff <= maxStepHeight
                 && canSee)
                {
                    a.neighbors.Add(b);
                    connected++;
                }
            }
            Debug.Log($"[ManagerNode] Nodo {a.name} tiene {connected} vecinos.");
        }
    }*/



    /// <summary>Devuelve solo los nodos de la zona dada</summary>
    public List<NodePathfinding> GetNodesInZone(int zoneId)
    {
        return nodes.Where(n => n.zoneId == zoneId).ToList();
    }

    public NodePathfinding GetClosestNode(Vector3 pos, int zoneId)
    {
        var zoneNodes = GetNodesInZone(zoneId);
        if (zoneNodes.Count == 0) return null;

        NodePathfinding closest = null;
        float minDist = Mathf.Infinity;
        foreach (var node in zoneNodes)
        {
            float d = Vector3.Distance(pos, node.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = node;
            }
        }
        return closest;
    }



    /// <summary>
    /// El nodo mas cercano al objetivo
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    /*public NodePathfinding NodeProx(Vector3 pos)
    {
        var disProx = Mathf.Infinity;
        NodePathfinding nearestNode = default;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (GameManager.instance.InLineOfSight(nodes[i].transform.position, pos)) //Pregunto si hay algo que interfiera entre el nodo y la pos del objetivo
            {
                var dis = pos - nodes[i].transform.position;

                if (dis.magnitude < disProx)
                {
                    disProx = dis.magnitude;
                    nearestNode = nodes[i];
                }
            }
        }

        return nearestNode;
    }*/

    public List<NodePathfinding> FindPath(NodePathfinding start, NodePathfinding goal)
    {
        Prioryti<NodePathfinding> frontier = new();
        frontier.Enqueue(start, 0);

        Dictionary<NodePathfinding, NodePathfinding> cameFrom = new();
        Dictionary<NodePathfinding, float> costSoFar = new();

        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            NodePathfinding current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (var neighbor in current.neighbors)
            {
                float newCost = costSoFar[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    float priority = newCost + Vector3.Distance(neighbor.transform.position, goal.transform.position);
                    frontier.Enqueue(neighbor, priority);
                    cameFrom[neighbor] = current;
                }
            }
        }

        // Reconstruir el camino
        List<NodePathfinding> path = new();
        NodePathfinding node = goal;

        while (node != null)
        {
            path.Add(node);
            cameFrom.TryGetValue(node, out node);
        }

        path.Reverse();
        return path;
    }

    public NodePathfinding GetClosestNode(Vector3 position)
    {
        NodePathfinding closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var node in nodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = node;
            }
        }

        return closest;
    }
}
