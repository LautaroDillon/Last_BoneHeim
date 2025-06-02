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

    // Gizmo cache
    private List<Vector3> gizmoNodePositions = new List<Vector3>();
    private List<(Vector3 from, Vector3 to)> gizmoConnections = new List<(Vector3, Vector3)>();

    [ContextMenu("Generate and Connect Nodes")]
    public void GenerateAndConnectNodes()
    {
        ClearOldNodes();
        generatedNodes.Clear();
        gizmoNodePositions.Clear();
        gizmoConnections.Clear();

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

                                // Cache gizmo position
                                gizmoNodePositions.Add(groundPos);
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
                if (node == otherNode) continue;

                Vector3 delta = otherNode.Position - node.Position;

                float horizontalDistance = new Vector2(delta.x, delta.z).magnitude;
                float verticalDifference = Mathf.Abs(delta.y);

                // Allow vertical steps (e.g. 0.5–1.2m) for stairs and check for clear connection
                if (horizontalDistance <= connectionRange && verticalDifference <= 1.2f)
                {
                    // Optional: Add a raycast to ensure clear walkable path (no ceiling)
                    Vector3 direction = (otherNode.Position - node.Position).normalized;
                    float distance = Vector3.Distance(node.Position + Vector3.up * 0.2f, otherNode.Position + Vector3.up * 0.2f);

                    if (!Physics.Raycast(node.Position + Vector3.up * 0.2f, direction, distance, obstacleMask))
                    {
                        if (!node.connections.Contains(otherNode))
                            node.connections.Add(otherNode);

                        gizmoConnections.Add((node.Position, otherNode.Position));
                    }
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
        gizmoNodePositions.Clear();
        gizmoConnections.Clear();
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

    public Node GetClosestGroundNode(Vector3 position)
    {
        Node closest = null;
        float closestDist = float.MaxValue;

        foreach (Node node in generatedNodes)
        {
            float dist = Vector3.Distance(position, node.Position);
            if (dist < closestDist)
            {
                closest = node;
                closestDist = dist;
            }
        }

        return closest;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.green;

        foreach (Vector3 pos in gizmoNodePositions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }

        foreach (var (from, to) in gizmoConnections)
        {
            Gizmos.DrawLine(from, to);
        }

        foreach (var (from, to) in gizmoConnections)
        {
            float yDiff = Mathf.Abs(from.y - to.y);
            Gizmos.color = yDiff > 0.2f ? Color.cyan : Color.green;
            Gizmos.DrawLine(from, to);
        }
    }
}
