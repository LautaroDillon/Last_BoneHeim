using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour
{
    Guns guns;
    public LootBag Bag;

    private void Start()
    {
        guns = GameObject.Find("Gun").GetComponent<Guns>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            guns.bulletsLeft += 1;
            Bag.intanceLoot(gameObject.transform.position);
        }
    }
}