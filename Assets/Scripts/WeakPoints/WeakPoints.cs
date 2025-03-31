using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoints : MonoBehaviour, Idamagable
{
    public string nameorg;
    public float damage;
    private void Awake()
    {
        nameorg = this.gameObject.tag;
    }

    public void TakeDamage(float dmg)
    {
        NormalSkeleton.instance.TakeDamage(damage);
        Debug.Log("ataco punto debil");
    }

    private void Start()
    {
        /*
        if (nameorg == FlyweightPointer.organs.Heart)
        {
            damage = FlyweightPointer.organs.heart;
            Debug.Log("tomo " + nameorg);
        }
        else if (nameorg == FlyweightPointer.organs.Higado)
        {
            damage = FlyweightPointer.organs.higado; 
            Debug.Log("tomo " + nameorg);
        }
        else if (nameorg == FlyweightPointer.organs.Pulmon)
        {
            damage = FlyweightPointer.organs.lung;
            Debug.Log("tomo " + nameorg);
        }
        */
    }

}
