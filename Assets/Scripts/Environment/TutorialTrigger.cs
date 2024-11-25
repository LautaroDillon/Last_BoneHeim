using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] RawImage rawVideo;
    [SerializeField] Button endVideo;

    private void Start()
    {
        rawVideo.gameObject.SetActive(false);
        endVideo.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rawVideo.gameObject.SetActive(true);
            endVideo.gameObject.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
