using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProyectiles : MonoBehaviour
{
    public float lifeTime = 2f;
    [SerializeField] private AudioClip bulletWallClip;
    [SerializeField] private AudioClip bulletHitClip;
    private void Awake()
    {
        Destroy(this.gameObject, lifeTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();

        if (other.gameObject.layer == 10 && damagableInterface != null)
        {
            Debug.Log("pego a enemigo");
            damagableInterface.TakeDamage(FlyweightPointer.Player.Damage);
            SoundManager.instance.PlaySound(bulletHitClip, transform, 0.3f);
            Destroy(this.gameObject);
        }
        else
        {
            SoundManager.instance.PlaySound(bulletWallClip, transform, 0.3f);
            Destroy(this.gameObject);
        }
    }

}
