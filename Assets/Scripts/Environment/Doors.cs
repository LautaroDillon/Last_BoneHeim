using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public GameObject limit;
    bool openDoor;
    public bool istab;
    
    public void Activate()
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

    private void Update()
    {
        if (openDoor)
        {
            transform.position = Vector3.MoveTowards(transform.position, limit.transform.position, 2 * Time.deltaTime);
        }
    }
}
