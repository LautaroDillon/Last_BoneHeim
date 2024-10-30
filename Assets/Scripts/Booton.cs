using UnityEngine;

public class Booton : MonoBehaviour
{
    public bool isbullet;
    public Doors door;
    public GameObject oclu;

    private void OnTriggerEnter(Collider other)
    {
        if (isbullet)
        {
            var whathit = other.gameObject.tag;

            if (whathit == "Bullet")
            {
                door.Activate();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isbullet)
        {
            var whathit = collision.gameObject.tag;

            if (whathit == "arm")
            {
                door.Activate();
            }
        }
    }
}
