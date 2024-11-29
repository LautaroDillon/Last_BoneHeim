using UnityEngine;

public class Booton1 : MonoBehaviour
{
    public bool isbullet;
    public Doors door;
    public GameObject oclu;
    public Animation hatchAnim;
    [SerializeField] private AudioClip dingClip;

    private void OnTriggerEnter(Collider other)
    {
        if (isbullet)
        {
            var whathit = other.gameObject.tag;

            if (whathit == "Arm")
            {
                door.Activate();
                hatchAnim.Play();
                SoundManager.instance.PlaySound(dingClip, transform, 1f, false);
            }
        }
    }

    public void invi()
    {
        oclu.SetActive(false);
    }
}
