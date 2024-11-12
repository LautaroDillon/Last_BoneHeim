using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, Idamagable
{
    public GameObject esto;
    public float _life;


    public void TakeDamage(float dmg)
    {
        _life -= dmg;
        if (_life <= 0)
        {
            esto.SetActive(false);
        }
    }
}
