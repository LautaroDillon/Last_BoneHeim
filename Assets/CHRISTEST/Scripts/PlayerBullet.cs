using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Variables")]
    private float counter;
    public float lifetime;
    void Update()
    {
        Lifetime();
    }
    public void Lifetime()
    {
        counter += Time.deltaTime;

        if (counter >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                Destroy(gameObject);
            }
            Destroy(gameObject);
        }

        
    }
}