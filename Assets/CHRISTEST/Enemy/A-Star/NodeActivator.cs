using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeActivator : MonoBehaviour
{
    void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
