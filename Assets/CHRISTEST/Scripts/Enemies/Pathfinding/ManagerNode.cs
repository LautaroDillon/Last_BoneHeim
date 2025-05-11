using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerNode : MonoBehaviour
{
    public static ManagerNode Instance;
    public List<NodePathfinding> nodes;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var currentNode = nodes[i];

            for (int j = 0; j < nodes.Count; j++)
            {
                if (nodes[j] == currentNode || currentNode.neighbors.Contains(nodes[j]))
                    continue;

                if (GameManager.instance.InLineOfSight(currentNode.transform.position, nodes[j].transform.position))
                    currentNode.neighbors.Add(nodes[j]);
            }
        }
    }

    /// <summary>
    /// El nodo mas cercano al objetivo
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public NodePathfinding NodeProx(Vector3 pos)
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
    }

    /*public void errorNodes()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
                var currentNode = nodes[i];

            if (currentNode.neighbors.Count < 0)
            {             
                for (int j = 0; j < nodes.Count; j++)
                {
                    if (nodes[j] == currentNode || currentNode.neighbors.Contains(nodes[j]))
                    continue;

                 if (GameManager.instance.InLineOfSight(currentNode.transform.position, nodes[j].transform.position))
                    currentNode.neighbors.Add(nodes[j]);
                }
            }
        }
    }*/
}
