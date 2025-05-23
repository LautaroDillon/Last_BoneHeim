using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject intactObject;
    [SerializeField] GameObject brokenObject;
    BoxCollider bc;
    CapsuleCollider cc;

    [Header("Sounds")]
    [SerializeField] private AudioClip breakClip;

    private void Awake()
    {
        intactObject.SetActive(true);
        brokenObject.SetActive(false);
        bc = GetComponent<BoxCollider>();
        cc = GetComponent<CapsuleCollider>();
    }

    private void Break()
    {
        Destroy(intactObject);
        brokenObject.SetActive(true);
        bc.enabled = false;
        cc.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bullet" || other.gameObject.tag == "EnemyBullet" || other.gameObject.tag == "Arm")
        {
            Break();
            Invoke("Destroy", 3);
            SoundManager.instance.PlaySound(breakClip, transform, 1f, false);
        }
    }

    void Destroy()
    {
        Destroy(this.gameObject);
    }
}
