using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public GameObject limit;
    protected bool openDoor;
    public bool istab;
    
    public virtual void Activate()
    {
        if (!istab)
        {
            openDoor = true;
        }
        else
        {
            openDoor = true;

        }
    }

    protected virtual void Update()
    {
        if (openDoor)
        {
            transform.position = Vector3.MoveTowards(transform.position, limit.transform.position, 2 * Time.deltaTime);
        }
    }
}
