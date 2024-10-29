using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booton : MonoBehaviour
{
    public bool isbullet;
    public Doors door;
    public GameObject oclu;

    private void OnTriggerEnter(Collider other)
    {
        var whathit = other.gameObject.tag;

        if (whathit == "Bullet" && isbullet)
        {
            door.Activate();

        }
        else
        {
            door.Activate();
        }
    }
}
