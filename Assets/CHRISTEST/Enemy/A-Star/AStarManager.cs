using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
            n.cameFrom = null;
        }

        start.gScore = 0;
        start.hScore = Vector3.Distance(start.transform.position, end.transform.position);

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = 0;
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                    lowestF = i;
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node> { end };
                while (currentNode != start)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }
                path.Reverse();
                return path;
            }

            foreach (Node neighbor in currentNode.connections)
            {
                float tentativeG = currentNode.gScore + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

                if (tentativeG < neighbor.gScore)
                {
                    neighbor.cameFrom = currentNode;
                    neighbor.gScore = tentativeG;
                    neighbor.hScore = Vector3.Distance(neighbor.transform.position, end.transform.position);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    public Node GetClosestNode(Vector3 position)
    {
        Node[] allNodes = FindObjectsOfType<Node>();
        Node closest = null;
        float minDist = Mathf.Infinity;

        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }

    public List<Vector3> FindPath(Vector3 from, Vector3 to)
    {
        Node start = GetClosestNode(from);
        Node end = GetClosestNode(to);

        var nodePath = GeneratePath(start, end);
        if (nodePath == null) return null;

        List<Vector3> path = new List<Vector3>();
        foreach (var node in nodePath)
        {
            path.Add(node.transform.position);
        }
        return path;
    }
}
