using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProyectiles : MonoBehaviour
{
    public static PlayerProyectiles instance;

    [Header("Variables")]
    public float lifeTime = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip bulletWallClip;
    [SerializeField] private AudioClip bulletHitClip;

    private void Awake()
    {
        Destroy(this.gameObject, lifeTime);
    }

    private void Start()
    {
        instance = this;
    }

    public void OnTriggerEnter(Collider other)
    {
        Idamagable damagableInterface = other.gameObject.GetComponent<Idamagable>();

        if (other.gameObject.layer == 10 && damagableInterface != null)
        {
            Debug.Log("Enemy Hit!");
            damagableInterface.TakeDamage(FlyweightPointer.Player.Damage);
            SoundManager.instance.PlaySound(bulletHitClip, transform, 0.3f, false);
            Destroy(this.gameObject);
            PlayerHealth.instance.life += PlayerHealth.instance.lifeSteal;
        }
        else
        {
            SoundManager.instance.PlaySound(bulletWallClip, transform, 0.3f, false);
            Destroy(this.gameObject);
        }
    }
}
