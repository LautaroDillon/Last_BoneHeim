using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ERockguardian : MonoBehaviour, Idamagable
{
    float currentLife;
    public float maxLife;
    public float speed;

    [Header("golpe tierra")]
    public GameObject smashEffect;
    public float smashRanged;
    public float smashCoolDown;
    float smashTimer;

    [Header("throw rock")]
    public GameObject rockPrefab;
    public Transform rockSpawnPoint;
    public float rockForce;
    public float rockCoolDown;
    float rocktimer;

    [Header("shield")]
    public GameObject shield;
    public float timeShield;
    public bool isShieldActivate;

    [Header("NavMesh")]
    public NavMeshAgent navMeshAgent;

    [Header("Player Detection")]
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected LayerMask obstructionMask;
    [SerializeField] protected Transform player;
    [SerializeField] public float checkRadius;
    [Range(0, 360)]
    public float angle;
    [SerializeField] protected bool canSeePlayer;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = maxLife;
        player = GameManager.instance.thisIsPlayer;
        navMeshAgent = GetComponent<NavMeshAgent>();
        isShieldActivate = false;

        StartCoroutine(FOVRoutime());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLife <= 0)
            return;

        float distancetoplayer = Vector3.Distance(transform.position, player.position);

        if (!isShieldActivate)
        {
            if (canSeePlayer)
            {
                if (distancetoplayer > smashRanged)
                {
                    MoveToPlayer();
                }
                else if (Time.time >= smashTimer + smashCoolDown)
                {
                    GroundSmash();
                }
                if (Time.time >= rocktimer + rockCoolDown)
                {
                    ThrowRock();
                }
            }
            else
            {
               // Patrol();
            }
        }
    }

    /*void Patrol()
    {
        if (navMeshAgent.enabled)
        {
            anim.SetBool("Punch", false);
            anim.SetBool("Walk", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Swiping", false);

            // Patrullar de manera aleatoria
            Vector3 randomDirection = Random.insideUnitSphere * 10f; // Generar un punto aleatorio
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, 1))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
    }*/

    public void resetAnim()
    {
        anim.SetBool("Punch", false);
        anim.SetBool("Idle", false);
        anim.SetBool("Swiping", false);
        anim.SetBool("Walking", false);
    }

    void MoveToPlayer()
    {
        if (navMeshAgent.enabled)
        {
            resetAnim();
            anim.SetBool("Walking", true);

            navMeshAgent.SetDestination(player.position);
        }
    }


    public void GroundSmash()
    {
        smashTimer = Time.time;
            resetAnim();
        anim.SetBool("Punch", true);

        Debug.Log("El Esqueleto usa Golpe de Tierra");
        //Instantiate(smashEffect, transform.position, Quaternion.identity);
        navMeshAgent.isStopped = true;

        // Aplica daño en un radio
        Collider[] hits = Physics.OverlapSphere(transform.position, smashRanged);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == 11)
            {
                hit.GetComponent<PlayerHealth>().TakeDamage(25);
            }
        }

        Invoke(nameof(ResumeMovement), 1.5f);
    }

    public void ThrowRock()
    {
        rocktimer = Time.time;
            resetAnim();
        anim.SetBool("Swiping", true);

        Debug.Log("El Esqueleto lanza una roca");

        navMeshAgent.isStopped = true;

        GameObject rock = Instantiate(rockPrefab, rockSpawnPoint.position, Quaternion.identity);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calcular la dirección del lanzamiento hacia el jugador
            Vector3 direction = (player.position - rockSpawnPoint.position).normalized;

            // Aplicar la fuerza a la roca en la dirección del jugador
            rb.AddForce(direction * rockForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("La roca lanzada no tiene Rigidbody");
        }

        Invoke(nameof(ResumeMovement), 2f);
    }

    private void ResumeMovement()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false;
        }
    }

    public void ActivateShield()
    {
        isShieldActivate = true;
        shield.SetActive(true);
        navMeshAgent.isStopped = true;

        Invoke("DeactivateShield", timeShield);
        Debug.Log("Escudo de Piedra activado");
    }

    private void DeactivateShield()
    {
        isShieldActivate = false;
        shield.SetActive(false);
        navMeshAgent.isStopped = false;
        Debug.Log("Escudo de Piedra desactivado");
    }

    public void TakeDamage(float damage)
    {
        if (isShieldActivate)
            currentLife -= damage / 2;
        else
            currentLife -= damage;


        if (currentLife <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El Esqueleto Guardián de Roca ha sido derrotado.");
        navMeshAgent.enabled = false;
        Destroy(gameObject);
    }

    private IEnumerator FOVRoutime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
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
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
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
