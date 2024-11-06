using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour
{
    public  GameObject point;
    public GameObject ally;
    public GameObject fuego;

    [SerializeField] private AudioClip thunderClip;

    bool activate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet" && !activate)
        {
            activate = true;
            if(activate)
            Interaction();
        }
    }

    public void Interaction()
    {
        Debug.Log("lala");
        SoundManager.instance.PlaySound(thunderClip, transform, 0.5f);
        fuego.SetActive(true);
        Instantiate(ally, point.transform.position, Quaternion.identity);
    }

    
}
