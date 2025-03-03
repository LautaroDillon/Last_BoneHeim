using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemisBehaivor : MonoBehaviour, Idamagable
{
    #region Variables
    public FSM fsm;
    protected NavMeshAgent agent;
    public EnemyType enemyType;

    public static EnemisBehaivor instance;
    [Header("References")]
    protected Guns gun;

    [Header("Variables")]
    [SerializeField] public float currentlife;
    [SerializeField] public float maxlife;
    [SerializeField] public float necroLife = 1500;
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

    [Header("Fsm")]
    public Node firstNode;

    #endregion

    private void Start()
    {
        player = GameManager.instance.thisIsPlayer;
        gun = GameObject.Find("Gun").GetComponent<Guns>();
        instance = this;
        StartCoroutine(FOVRoutime());

        maxlife = currentlife;

        GameManager.instance.AddToList(this.gameObject);

        /* fsm = new FSM();
         fsm.CreateState("Attack", new AttackEnemy(fsm, this));
         fsm.CreateState("Escape", new Escape(fsm, this));
         fsm.CreateState("Walk", new Walk(fsm, this));
         fsm.ChangeState("Walk");*/
    }

    public virtual void AttackPlayer() { }

    public virtual void Patrol() { }

    public virtual void Escape() { }

    #region Life
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
            if(gun != null)
            Guns.instance.bulletsLeft += Random.Range(1, 3) + gun.killReward;
        }
    }

    public void Healing(int heal)
    {
        healParticle.SetActive(true);
        if (currentlife <= maxlife)
        {
            currentlife += heal;
        }
        else
        {
            return;
        }
    }
    #endregion

    #region Fov
    public virtual IEnumerator FOVRoutime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
       // Debug.Log("busco player");

        while (true)
        {
            yield return wait;
            canSeePlayer = FieldOfViewCheck();
        }
    }
    public virtual bool FieldOfViewCheck()
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
                   // Debug.Log("veo player");
                    return true;
                }
                else
                {
                   // Debug.Log(" no veo player");

                    return false;
                }
            }
            else
            {
                    //Debug.Log(" no veo player");
                return false;
            }
        }
        else
        {
                    //Debug.Log(" no veo player");
            return false;
        }
    }
    #endregion

    #region MoveHealer
    public bool CanReachTarget(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasPath = agent.CalculatePath(targetPosition, path);

        if (hasPath && path.status == NavMeshPathStatus.PathComplete)
        {
            return true; // Hay un camino válido sin obstáculos
        }

        Debug.Log("No se puede alcanzar al sanador, el camino está bloqueado.");
        return false; // No hay un camino claro en el NavMesh
    }

    public EnemisBehaivor FindClosestHealer()
    {
        EnemisBehaivor closestHealer = null;
        float minDistance = Mathf.Infinity;

        foreach (EnemisBehaivor enemy in GameManager.instance.Healers)
        {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance < minDistance && CanReachTarget(enemy.transform.position)) // Verifica si el camino está libre en NavMesh
                {
                    minDistance = distance;
                    closestHealer = enemy;
                }           
        }
        return closestHealer;
    }

    public void MoveToHealer()
    {
        EnemisBehaivor healer = FindClosestHealer();
        if (healer != null)
        {
            agent.SetDestination(healer.transform.position);
            fsm.ChangeState("Escape"); // Cambia a estado de escape hacia el sanador
        }
        else
        {
            Debug.Log("No hay sanadores disponibles o el camino está bloqueado, huyendo normalmente.");
            FleeFromPlayer();
        }
    }

    public virtual void FleeFromPlayer()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 fleePosition = transform.position + directionAwayFromPlayer.normalized * 10f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        resetAnim();
        anim.SetBool("Moving", true);
    }
    #endregion

    public virtual void resetAnim() { }

    public virtual bool HasEnoughNearbyAllies() { return false; }

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

public enum EnemyType
{
    atack,
    Healer
}
