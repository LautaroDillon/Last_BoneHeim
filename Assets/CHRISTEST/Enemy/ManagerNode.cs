using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerNode : MonoBehaviour
{
    public static ManagerNode Instance;
    public List<NodePathfinding> nodes;

    [Header("Connection Settings")]
    public float maxNeighborDistance = 3f;
    public float maxStepHeight = 1.5f;

    private void Awake()
    {
        Instance = this;
        nodes = FindObjectsOfType<NodePathfinding>().ToList();
        Debug.Log($"[ManagerNode] Loaded {nodes.Count} nodes");
    }

    #region Zone Helpers

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
            float dist = Vector3.Distance(pos, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
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

    #endregion

    #region Pathfinding (A*)

    public List<NodePathfinding> FindPath(NodePathfinding start, NodePathfinding goal)
    {
        var frontier = new Priority<NodePathfinding>();
        var cameFrom = new Dictionary<NodePathfinding, NodePathfinding>();
        var costSoFar = new Dictionary<NodePathfinding, float>();

        frontier.Enqueue(start, 0);
        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            NodePathfinding current = frontier.Dequeue();
            if (current == goal) break;

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

        // Reconstruct path
        var path = new List<NodePathfinding>();
        var node = goal;

        while (node != null)
        {
            path.Add(node);
            cameFrom.TryGetValue(node, out node);
        }

        path.Reverse();
        return path;
    }

    #endregion

    #region Optional Neighbor Auto-Connection (Disabled by default)

    /*
    private void Start()
    {
        foreach (var a in nodes)
        {
            int connected = 0;
            foreach (var b in nodes)
            {
                if (a == b) continue;

                float planarDist = Vector3.Distance(new Vector3(a.transform.position.x, 0, a.transform.position.z),
                                                    new Vector3(b.transform.position.x, 0, b.transform.position.z));
                float heightDiff = Mathf.Abs(a.transform.position.y - b.transform.position.y);

                bool canSee = GameManager.instance.InLineOfSight(
                    a.transform.position + Vector3.up * 0.5f,
                    b.transform.position + Vector3.up * 0.5f
                );

                if (planarDist <= maxNeighborDistance &&
                    heightDiff <= maxStepHeight &&
                    canSee)
                {
                    a.neighbors.Add(b);
                    connected++;
                }
            }

            Debug.Log($"[ManagerNode] Node {a.name} connected to {connected} neighbors.");
        }
    }
    */

    #endregion
}
