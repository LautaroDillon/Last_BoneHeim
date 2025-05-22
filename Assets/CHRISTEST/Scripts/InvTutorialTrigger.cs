using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvTutorialTrigger : MonoBehaviour
{
    TutorialTrigger tutTrigger;
    public GameObject door;
    public bool canOpenDoor;
    void Start()
    {
        canOpenDoor = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canOpenDoor == true)
        {
            if(Input.GetKeyDown(KeyCode.Tab))
                door.transform.Translate(Vector3.up * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            canOpenDoor = true;
            print("Can open door!");
        }
    }
}
