using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    public GameObject limit;
    bool openDoor;

    public void Activate()
    {
        openDoor = true;
    }

    private void Update()
    {
        if (openDoor)
        {
            transform.position = Vector3.MoveTowards(transform.position, limit.transform.position, 2 * Time.deltaTime);
        }
    }
}
