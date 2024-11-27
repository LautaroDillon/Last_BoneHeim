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
    public Transform player;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = maxLife;
        player = GameManager.instance.thisIsPlayer;
        navMeshAgent = GetComponent<NavMeshAgent>();
        isShieldActivate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLife <= 0)
            return;

        float distancetoplayer = Vector3.Distance(transform.position, player.position);

        if (!isShieldActivate)
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
    }

    void MoveToPlayer()
    {
        if (navMeshAgent.enabled)
        {
            anim.SetBool("Punch", false);
            anim.SetBool("Walk", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Swiping", false);

            navMeshAgent.SetDestination(player.position);
        }
    }


    public void GroundSmash()
    {
        smashTimer = Time.time;
        anim.SetBool("Idle", false);
        anim.SetBool("Punch", true);
        anim.SetBool("Walk", false);
        anim.SetBool("Swiping", false);
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
        anim.SetBool("Punch", false);
        anim.SetBool("Walk", false);
            anim.SetBool("Idle", false);
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

        Invoke(nameof(ResumeMovement), 1f);
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
}
