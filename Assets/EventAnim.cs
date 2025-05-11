using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnim : MonoBehaviour
{
    static EventAnim instance;

    public GameObject PoitnsOrgan;
    public GameObject organs;

    private void Start()
    {
        instance = this;
        organs.SetActive(false);
    }

    private void Update()
    {
        organs.transform.position = PoitnsOrgan.transform.position;
    }

    public void HeartEventActive()
    {
        organs.transform.position = PoitnsOrgan.transform.position;
        organs.SetActive(true);
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        organs.SetActive(false);
    }
}
