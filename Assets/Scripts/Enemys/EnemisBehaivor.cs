using UnityEngine;

public class EnemisBehaivor : MonoBehaviour, Idamagable
{
    public static EnemisBehaivor instance;


    [SerializeField] protected float currentlife;
    [SerializeField] protected float speed;

    [Header("Detecion del player")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected bool IsInChaseRange;
    [SerializeField] protected Transform player;
    [SerializeField] public float checkRadius;

    [Header("movimiento")]
    protected int rutina;
    [SerializeField] protected float cronometro;
    protected Quaternion angulo;
    protected float grado;
    [SerializeField] protected float ranged;

    Guns gun;
    [SerializeField] protected AudioClip deathClip;

    [Header("particulas")]
    [SerializeField] protected GameObject blood;
    [SerializeField] protected GameObject skeletaldamage;
    [SerializeField] protected GameObject pointParticle;
    [SerializeField] public GameObject healParticle;

    [SerializeField] protected Animator anim;

    private void Start()
    {
        player = GameManager.instance.thisIsPlayer;
        gun = GameObject.Find("Gun").GetComponent<Guns>();
        instance = this;

        GameManager.instance.AddToList(this.gameObject);
    }

    public virtual void TakeDamage(float dmg)
    {
        healParticle.SetActive(false);

        currentlife -= dmg;
        GameObject acid = Instantiate(blood, pointParticle.transform.position, Quaternion.identity);
        GameObject debris = Instantiate(skeletaldamage, pointParticle.transform.position, Quaternion.identity);

        Destroy(debris, 5);
        Destroy(acid, 15);

        if (currentlife <= 0)
        {
            Debug.Log("the skeleton received damage ");
            GameManager.instance.enemys.Remove(this.gameObject);

            SoundManager.instance.PlaySound(deathClip, transform, 0.7f, false);

            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }

            Destroy(acid);
            Destroy(this.gameObject, 0.1f);
            PlayerHealth.instance.life += 10;
            Guns.instance.bulletsLeft += Random.Range(4, 5) + gun.killReward;
        }
    }

    public void detection()
    {
        IsInChaseRange = Physics.CheckSphere(transform.position, checkRadius, whatIsPlayer);
    }

    public void Healing(int heal)
    {
        healParticle.SetActive(true);
        if (currentlife <= 100)
        {
            currentlife += heal;
        }
        else
        {
            currentlife = 100;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ranged);
    }
}
