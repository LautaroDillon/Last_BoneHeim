using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeConnector : MonoBehaviour
{
    public float connectionRange = 2.5f;

    [ContextMenu("Connect All Nodes")]
    public void ConnectAllNodes()
    {
        Node[] nodes = FindObjectsOfType<Node>();

        foreach (Node node in nodes)
        {
            node.connections.Clear();

            foreach (Node other in nodes)
            {
                if (node != other && Vector3.Distance(node.Position, other.Position) <= connectionRange)
                {
                    if (!node.connections.Contains(other))
                        node.connections.Add(other);
                }
            }
        }
    }
}
