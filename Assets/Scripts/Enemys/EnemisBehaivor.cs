using UnityEngine;

public class EnemisBehaivor : MonoBehaviour, Idamagable
{
    [SerializeField] protected float currentlife;
    [SerializeField] protected float speed;

    [Header("Detecion del player")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected bool IsInChaseRange;
    [SerializeField] protected Transform player;

    [Header("movimiento")]
    protected int rutina;
    [SerializeField] protected float cronometro;
    protected Quaternion angulo;
    protected float grado;
    [SerializeField] protected float ranged;

    Guns gun;
    [SerializeField] private AudioClip deathClip;

    [Header("particulas")]
    [SerializeField] protected GameObject blood;
    [SerializeField] protected GameObject pointParticle;

    // [SerializeField] Animator anim;
    private void Start()
    {
        player = GameManager.instance.thisIsPlayer;
        gun = GameObject.Find("Gun").GetComponent<Guns>();

        GameManager.instance.AddToList(this.gameObject);
    }

    public virtual void TakeDamage(float dmg)
    {
        currentlife -= dmg;
        GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);

        Destroy(acid, 3);

        if (currentlife <= 0)
        {
            Debug.Log("the skeleton received damage ");
            GameManager.instance.enemys.Remove(this.gameObject);

            SoundManager.instance.PlaySound(deathClip, transform, 0.7f);

            Destroy(acid);
            Destroy(this.gameObject, 0.1f);

            Guns.instance.bulletsLeft += Random.Range(1, 3) + gun.killReward;
        }
    }

    public void Healing(int heal)
    {
        if (currentlife <= 100)
        {
            currentlife += heal;
        }
        else
        {
            currentlife = 100;
        }
    }
}
