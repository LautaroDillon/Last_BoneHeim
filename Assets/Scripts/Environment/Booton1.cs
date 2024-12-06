using UnityEngine;

public class Booton1 : MonoBehaviour
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
    private void OnTriggerEnter(Collider other)
    {
        if(alreadyActivated == false)
        {
            if (isbullet)
            {
                var whathit = other.gameObject.tag;

                if (whathit == "Arm")
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
