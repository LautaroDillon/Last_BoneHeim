using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections = new List<Node>();

    public float gScore;
    public float hScore;

    public float FScore()
    {
        return gScore + hScore;
    }

    public Vector3 Position => transform.position;

    private void OnDrawGizmos()
    {
        NodeGrid manager = FindObjectOfType<NodeGrid>();
        if (manager == null || !manager.showGizmos)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.blue;
        if (connections != null)
        {
            foreach (var connection in connections)
            {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
        }
    }
}
