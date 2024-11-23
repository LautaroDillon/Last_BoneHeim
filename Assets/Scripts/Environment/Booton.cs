using UnityEngine;

public class Booton : MonoBehaviour
{
    public bool isbullet;
    public Doors door;
    public GameObject oclu;
    [SerializeField] private AudioClip dingClip;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            door.Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isbullet)
        {
            var whathit = other.gameObject.tag;

            if (whathit == "Bullet")
            {
                door.Activate();
                SoundManager.instance.PlaySound(dingClip, transform, 1f, false);
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
                SoundManager.instance.PlaySound(dingClip, transform, 1f, false);
            }
        }
    }

    public void invi()
    {
        oclu.SetActive(false);
    }
}
