using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAnim : MonoBehaviour
{
    public static EventAnim instance;

    public GameObject organPoint;
    public GameObject organs;
    private bool isFollowing = false;

    private void Start()
    {
        instance = this;
        organs.SetActive(false);
    }

    public void HeartEventActive()
    {
        organs.SetActive(true);
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        organs.transform.SetParent(null);
        organs.SetActive(false);
    }
}
