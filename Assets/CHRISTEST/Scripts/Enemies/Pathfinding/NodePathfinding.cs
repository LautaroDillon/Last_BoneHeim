using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePathfinding : MonoBehaviour
{
    public List<NodePathfinding> neighbors = new List<NodePathfinding>();

    public int cost;

    private void Awake()
    {
        if (ManagerNode.Instance.nodes.Contains(this))
            Debug.LogWarning("Node already exists in the list");
        else
            ManagerNode.Instance.nodes.Add(this);
    }

/*#if neighbors == null

    private void Update()
    {
        ManagerNode.Instance.errorNodes();
    }




#endif*/
}
