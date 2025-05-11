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

    private void Update()
    {
        if (isFollowing && organPoint != null && organs != null)
        {
            organs.transform.position = Vector3.Lerp(
                organs.transform.position,
                organPoint.transform.position + new Vector3(0.30f, 0.02f, 0.1f),
                Time.deltaTime * 5f
            );
            organs.transform.rotation = Quaternion.Slerp(
                organs.transform.rotation,
                organPoint.transform.rotation * Quaternion.Euler(0, 90, 0),
                Time.deltaTime * 5f
            );
        }
    }

    public void HeartEventActive()
    {
        organs.SetActive(true);
        isFollowing = true;
    }

    public void HeartEventDeactive()
    {
        Debug.Log("Deactive");
        isFollowing = false;
        organs.transform.SetParent(null);
        organs.SetActive(false);
    }
}
