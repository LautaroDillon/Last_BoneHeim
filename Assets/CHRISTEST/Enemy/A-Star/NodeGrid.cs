using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public GameObject nodePrefab;

    public LayerMask groundMask;
    public LayerMask obstacleMask;

    public Collider[] allowedAreas;

    public float spacing = 1f;
    public float nodeCheckRadius = 0.4f;
    public float connectionRange = 1.5f;

    private List<Node> generatedNodes = new List<Node>();
    public List<Node> GeneratedNodes => generatedNodes;
    public bool showGizmos = true;

    [ContextMenu("Generate and Connect Nodes")]
    public void GenerateAndConnectNodes()
    {
        ClearOldNodes();
        generatedNodes.Clear();

        if (allowedAreas == null || allowedAreas.Length == 0)
        {
            Debug.LogWarning("No allowed areas assigned for node generation!");
            return;
        }

        foreach (var area in allowedAreas)
        {
            if (area == null) continue;

            Bounds areaBounds = area.bounds;
            Vector3 size = areaBounds.size;
            Vector3 center = areaBounds.center;

            int gridX = Mathf.FloorToInt(size.x / spacing);
            int gridZ = Mathf.FloorToInt(size.z / spacing);

            Vector3 startPos = new Vector3(
                center.x - size.x / 2f + spacing / 2f,
                center.y + 10f,
                center.z - size.z / 2f + spacing / 2f
            );

            for (int x = 0; x < gridX; x++)
            {
                for (int z = 0; z < gridZ; z++)
                {
                    Vector3 rayStart = startPos + new Vector3(x * spacing, 0, z * spacing);

                    if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 20f, groundMask))
                    {
                        Vector3 groundPos = hit.point;

                        if (areaBounds.Contains(groundPos) && !Physics.CheckSphere(groundPos, nodeCheckRadius, obstacleMask))
                        {
                            GameObject nodeObj = Instantiate(nodePrefab, groundPos, Quaternion.identity, transform);
                            nodeObj.SetActive(false);

                            Node node = nodeObj.GetComponent<Node>();
                            if (node != null)
                            {
                                generatedNodes.Add(node);
                                node.connections.Clear();
                            }
                        }
                    }
                }
            }
        }

        foreach (Node node in generatedNodes)
        {
            foreach (Node otherNode in generatedNodes)
            {
                if (node != otherNode && Vector3.Distance(node.Position, otherNode.Position) <= connectionRange)
                {
                    if (!node.connections.Contains(otherNode))
                        node.connections.Add(otherNode);
                }
            }
        }

        Debug.Log($"Generated {generatedNodes.Count} nodes inside allowed areas.");
    }

    [ContextMenu("Clear Nodes")]
    public void ClearNodes()
    {
        ClearOldNodes();
        generatedNodes.Clear();
        Debug.Log("All nodes cleared.");
    }

    private void ClearOldNodes()
    {
        Node[] oldNodes = GetComponentsInChildren<Node>(true);
        foreach (Node node in oldNodes)
        {
            DestroyImmediate(node.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || generatedNodes == null) return;

        Gizmos.color = Color.green;
        foreach (Node node in generatedNodes)
        {
            Gizmos.DrawSphere(node.transform.position, 0.1f);

            if (node.connections != null)
            {
                foreach (Node conn in node.connections)
                {
                    Gizmos.DrawLine(node.transform.position, conn.transform.position);
                }
            }
        }
    }
}
