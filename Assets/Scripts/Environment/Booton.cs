using UnityEngine;

public class Booton : MonoBehaviour
{
    public bool isbullet;
    private bool alreadyActivated;
    public Doors door;
    public GameObject oclu;
    public Animation hatchAnim;
    public ParticleSystem _particle;
    [SerializeField] private AudioClip dingClip;

    private void Awake()
    {
        alreadyActivated = false;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            door.Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(alreadyActivated == false)
        {
            if (isbullet)
            {
                var whathit = other.gameObject.tag;

                if (whathit == "Bullet")
                {
                    alreadyActivated = true;
                    door.Activate();
                    hatchAnim.Play();
                    _particle.Play();
                    SoundManager.instance.PlaySound(dingClip, transform, 1f, false);
                }
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if(alreadyActivated == false)
        {
            if (!isbullet)
            {
                var whathit = collision.gameObject.tag;

                if (whathit == "arm")
                {
                    alreadyActivated = true;
                    door.Activate();
                    hatchAnim.Play();
                    _particle.Play();
                    SoundManager.instance.PlaySound(dingClip, transform, 1f, false);
                }
            }
        }
        
    }

    public void invi()
    {
        oclu.SetActive(false);
    }
}
