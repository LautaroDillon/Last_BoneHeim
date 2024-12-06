using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemisBehaivor : MonoBehaviour, Idamagable
{
    #region Variables
    public static EnemisBehaivor instance;
    [Header("References")]
    protected Guns gun;

    [Header("Variables")]
    [SerializeField] public float currentlife;
    [SerializeField] protected float speed;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected LayerMask obstructionMask;
    [SerializeField] protected Transform player;
    [SerializeField] protected float checkRadius;
    [Range(0,360)]
    [SerializeField] protected float angle;
    [SerializeField] protected bool canSeePlayer;

    [Header("Movement")]
    protected int rutina;
    [SerializeField] protected float cronometro;
    protected Quaternion angulo;
    protected float grado;
   // [SerializeField] protected float ranged;

    [Header("Sounds")]
    [SerializeField] protected AudioClip skeletonDeathClip;
    [SerializeField] protected AudioClip boomerDeathClip;
    [SerializeField] protected AudioClip necromancerDeathClip;
    [SerializeField] protected AudioClip invokerDeathClip;
    [SerializeField] protected AudioClip chamanDeathClip;

    [Header("Particles")]
    [SerializeField] protected GameObject blood;
    [SerializeField] protected GameObject skeletaldamage;
    [SerializeField] protected GameObject pointParticle;
    [SerializeField] public GameObject healParticle;

    [Header("Animation")]
    [SerializeField] protected Animator anim;
    #endregion

    private void Start()
    {
        player = GameManager.instance.thisIsPlayer;
        gun = GameObject.Find("Gun").GetComponent<Guns>();
        instance = this;
        StartCoroutine(FOVRoutime());

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

            if(gameObject.tag == "Skeleton")
                SoundManager.instance.PlaySound(skeletonDeathClip, transform, 1f, false);
            if(gameObject.tag == "Boomer")
                SoundManager.instance.PlaySound(boomerDeathClip, transform, 1f, false);
            if(gameObject.tag == "Necromancer")
                SoundManager.instance.PlaySound(necromancerDeathClip, transform, 1f, false);
            if(gameObject.tag == "Invoker")
                SoundManager.instance.PlaySound(invokerDeathClip, transform, 1f, false);
            if(gameObject.tag == "Chaman")
                SoundManager.instance.PlaySound(chamanDeathClip, transform, 1f, false);

            if (PlayerHealth.instance.isInReviveState)
            {
                PlayerHealth.instance.enemyKilled = true;
            }

            Destroy(acid);
            Destroy(this.gameObject, 0.1f);
            PlayerHealth.instance.life += 10;
            Guns.instance.bulletsLeft += Random.Range(1, 3) + gun.killReward;
        }
    }

    public void detection()
    {
       // IsInChaseRange = Physics.CheckSphere(transform.position, checkRadius, whatIsPlayer);
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

    public virtual IEnumerator FOVRoutime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        Debug.Log("busco player");

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    public virtual void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, checkRadius, whatIsPlayer);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    Debug.Log("veo player");
                    canSeePlayer = true;
                }
                else
                {
                    Debug.Log(" no veo player");

                    canSeePlayer = false;
                }
            }
            else
            {
                    Debug.Log(" no veo player");
                canSeePlayer = false;
            }
        }
        else
        {
                    Debug.Log(" no veo player");
            canSeePlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, checkRadius);

        Gizmos.color = Color.yellow;
        Vector3 fovLine1 = DirFromAngle(-angle / 2, false);
        Vector3 fovLine2 = DirFromAngle(angle / 2, false);

        Gizmos.DrawLine(transform.position, transform.position + fovLine1 * checkRadius);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2 * checkRadius);
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
