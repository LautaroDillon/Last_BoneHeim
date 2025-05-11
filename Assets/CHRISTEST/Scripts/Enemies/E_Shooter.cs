using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class E_Shooter : Entity
{
    #region variables
    public StateMachine fsm;

    [Header("References")]
    public Transform player;
    public Animator anim;
    public LayerMask whatIsGround, whatIsPlayer;
    public static E_Shooter instance;
    public GameObject bulletDrop;
    public int numberOfBulletsOnDeath;
    public PlayerWeapon playerWeapon;

    [Header("Attack")]
    public float shotCooldown;
    public bool alreadyAttacked;
    public GameObject firePoint;
    [SerializeField] private float projectileSpeed;
    public float strafeSpeed = 5f;
    public bool wasKilledByThrowable = false;

    [Header("States")]
    public float chaseDistance, attackRange;
    public bool playerInSightRange, playerInAttackRange, canSeePlayer;
    public bool isIdle = true;
    public bool WasHit;
    public bool isPatrolling;
    public float muchit;

    [Header("Player Detection")]
    [SerializeField] public LayerMask obstructionMask;
    [SerializeField] protected float checkRadius;
    [Range(0, 360)]
    [SerializeField] protected float angle;
    public Vector3 lastpoint;

    [Header("Nodes")]
    public NodePathfinding initialNode;
    public NodePathfinding goalNode;
    public List<NodePathfinding> path;
    public int pathIndex;
    public float nodeReachDistance = 0.3f;

    public float moveSpeed;
    public float maxSpeed;
    public float arriveRadius;
    public float maxForce;
    [HideInInspector] public Vector3 velocity;

    [Header("Zona")]
    public int zoneId;

    #endregion
    public bool isincombatArena;

    private void Awake()
    {
        maxHealth = EnemyFlyweight.Shooter.maxLife;
        currentHealth = maxHealth;
        numberOfBulletsOnDeath = Random.Range(1, 4);
        fsm = new StateMachine();
        if (instance == null)
            instance = this;
    }

    #region start
    private void Start()
    {
        playerWeapon = FindObjectOfType<PlayerWeapon>();

        StartCoroutine(FOVRoutime());

        var idle = new Idle( this, fsm);
        var patrol = new Patrol( this, fsm);
        var chase = new Chase( this, fsm);
        var Search = new Serach_S( this, fsm);
        var strafe = new Strafe( this, fsm);
        var attack = new Atack( this, fsm);
        var death = new Death( this, fsm);
        var Onhit = new OnHit( this, fsm);


        // Definir las transiciones
        at(idle, patrol, () => !isIdle && !isDead);
        at(patrol, idle, () => !isPatrolling && !isDead);
        at(patrol, chase, () => canSeePlayer && !playerInAttackRange && !isDead);
        at(chase, attack, () => playerInAttackRange && !isDead);
        at(attack, strafe, () => alreadyAttacked && playerInAttackRange && !isDead);
        at(attack, chase, () => !playerInAttackRange && !isDead);
        at(attack, Search, () => !playerInAttackRange && !canSeePlayer && !isDead);
        at(strafe, chase, () => !playerInAttackRange && !isDead);
        at(strafe, attack, () => !alreadyAttacked && !isDead);
        at(chase, Search, () => !canSeePlayer && !isDead);
        at(Search, patrol, () => true && !isDead); // Despues de buscar vuelve a patrullar
        any(death, () => currentHealth <= 0 && isDead); // Transición a Death desde cualquier estado
        any(Onhit, () => WasHit && !isDead); // Transición a hit desde cualquier estado
        at(Onhit, idle, () => !WasHit && !isDead); // Transición a idle desde hit
        at(Onhit, patrol, () => !WasHit && !isDead);
        at(Onhit, chase, () => !WasHit && !isDead);
        at(Onhit, Search, () => !WasHit && !isDead);
        at(Onhit, strafe, () => !WasHit && !isDead);
        at(Onhit, attack, () => !WasHit && !isDead); 

        fsm.SetState(idle);
    }


    void at(IState from, IState to, Func<bool> condition) => fsm.AddTransition(from, to, condition);
    void any(IState to, Func<bool> condition) => fsm.AddAnyTransition(to, condition);
    #endregion

    private void Update()
    {
        fsm.Tick();
    }

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
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
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

    /*  #region Movement
      public Vector3 Seek(Vector3 targetSeek)
      {
          var desired = targetSeek - transform.position;
          desired.Normalize();
          desired *= maxSpeed;
          return CalculateSteering(desired);
      }
      public Vector3 CalculateSteering(Vector3 desired)
      {
          var steering = desired - velocity;
          steering = Vector3.ClampMagnitude(steering, maxForce);
          return steering;
      }

      public void AddForce(Vector3 dir)
      {
          velocity += dir;
          //velocity.y = transform.position.y; //Mantengo mi altura
          velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
      }

      public Vector3 ObstacleAvoidance()
      {
          Vector3 pos = transform.position;
          Vector3 dir = transform.forward;
          float dist = velocity.magnitude; //Que tan rapido estoy yendo

          Debug.DrawLine(pos, pos + (dir * dist));

          if (Physics.SphereCast(pos, 1, dir, out RaycastHit hit, dist, obstructionMask))
          {
              var obstacle = hit.transform; //Obtengo el transform del obstaculo q acaba de tocar
              Vector3 dirToObject = obstacle.position - transform.position; //La direccion del obstaculo

              float anguloEntre = Vector3.SignedAngle(transform.forward, dirToObject, Vector3.up); //(Dir. hacia donde voy, Dir. objeto, Dir. mis costados)

              Vector3 desired = anguloEntre >= 0 ? -transform.right : transform.right; //Me meuvo para derecha o izquierda dependiendo donde esta el obstaculo
              desired.Normalize();
              desired *= maxSpeed;

              return CalculateSteering(desired);
          }

          return Vector3.zero;
      }

      public List<NodePathfinding> CalculateAStar(NodePathfinding startingNode, NodePathfinding goalNode)
      {
          Debug.Log("Calculando A* desde " + startingNode.name + " a " + goalNode.name);
          Prioryti<NodePathfinding> frontier = new Prioryti<NodePathfinding>();
          frontier.Enqueue(startingNode, 0);

          Dictionary<NodePathfinding, NodePathfinding> cameFrom = new Dictionary<NodePathfinding, NodePathfinding>();
          cameFrom.Add(startingNode, null);

          Dictionary<NodePathfinding, int> costSoFar = new Dictionary<NodePathfinding, int>();
          costSoFar.Add(startingNode, 0);

          while (frontier.Count > 0)
          {
              NodePathfinding current = frontier.Dequeue();

              if (current == goalNode)
              {
                  List<NodePathfinding> path = new List<NodePathfinding>();

                  while (current != startingNode)
                  {
                      path.Add(current);
                      current = cameFrom[current];
                  }

                  path.Reverse();
                  return path;
              }

              foreach (var item in current.neighbors)
              {

                  int newCost = costSoFar[current] + item.cost; //Calculo el costo como en Dijkstra
                  float priority = newCost + Vector3.Distance(item.transform.position, goalNode.transform.position); //Calculo la distancia del nodo actual hasta la meta

                  if (!costSoFar.ContainsKey(item))
                  {
                      if (!frontier.ContainsKey(item))
                          frontier.Enqueue(item, priority);
                      cameFrom.Add(item, current);
                      costSoFar.Add(item, newCost);
                  }
                  else if (costSoFar[item] > newCost)
                  {
                      if (!frontier.ContainsKey(item))
                          frontier.Enqueue(item, priority);
                      cameFrom[item] = current;
                      costSoFar[item] = newCost;
                  }
              }
          }
          return new List<NodePathfinding>();
      }

      public List<NodePathfinding> CalculateThetaStar(NodePathfinding startingNode, NodePathfinding goalNode) //Me borra los nodos q estan de más en el recorrido
      {
          Debug.Log("Calculando Theta* desde " + startingNode.name + " a " + goalNode.name);
          var listNode = CalculateAStar(startingNode, goalNode); //Llamo a AStar

          int current = 0;

          while (current + 2 < listNode.Count)
          {
              if (GameManager.instance.InLineOfSight(listNode[current].transform.position, listNode[current + 2].transform.position)) //Si puedo llegar a un nodo siguiente
              {
                  listNode.RemoveAt(current + 1); //Borro el anterior nodo
              }
              else
                  current++; //Sino me lo sumo
          }

          return listNode;
      }
      #endregion*/


    public void MoveAlongPath()
    {
        if (path == null || pathIndex >= path.Count) return;

        NodePathfinding targetNode = path[pathIndex];
        Vector3 direction = (targetNode.transform.position - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.deltaTime;

        transform.position += move;

        // Rotación suave hacia el siguiente nodo
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

        // Animaciones
        anim.SetFloat("Horizontal", direction.x, 0.2f, Time.deltaTime);
        anim.SetFloat("Vertical", direction.z, 0.2f, Time.deltaTime);

        if (Vector3.Distance(transform.position, targetNode.transform.position) < nodeReachDistance)
            pathIndex++;
    }

    public void CalculatePathToRandomNode()
    {
        var start = ManagerNode.Instance.GetClosestNode(transform.position);
        var allNodes = ManagerNode.Instance.nodes;

        NodePathfinding randomTarget = null;
        int attempts = 0;

        while ((randomTarget == null || randomTarget == start) && attempts < 10)
        {
            randomTarget = allNodes[Random.Range(0, allNodes.Count)];
            attempts++;
        }

        path = ManagerNode.Instance.FindPath(start, randomTarget);
        pathIndex = 0;
    }


    #region takedamage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0 && currentHealth <= (maxHealth/3))
        {
            WasHit = true;
        }
        if (currentHealth <= 0 && !isincombatArena)
        {
            isDead = true;

        }
    }
    public void Death()
    {
        if (isDead == true)
        {
            AudioManager.instance.PlaySFXOneShot("ShooterDeath", 1f);
            DropBullets();
            Invoke("DestroyEnemy", 2.3f);
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    #endregion

    void DropBullets()
    {
        for (int i = 0; i < numberOfBulletsOnDeath; i++)
        {
            Vector3 dropOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                0.5f,
                Random.Range(-0.3f, 0.3f)
            );

            Vector3 dropPosition = transform.position + dropOffset;

            GameObject bullet = Instantiate(bulletDrop, dropPosition, Quaternion.identity);

            bullet.transform.position += Vector3.up * 0.3f;

            // Add physics force in random arc
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    1f,
                    Random.Range(-1f, 1f)
                ).normalized;

                float dropForce = Random.Range(3f, 6f);
                rb.AddForce(randomDirection * dropForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance); 
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }


    public IEnumerator waitforsecond(float time)
    {
        AudioManager.instance.PlaySFXOneShot("ShooterDamage", 1f);
        yield return new WaitForSeconds(time);
        WasHit = false;
    } 
    
    public IEnumerator waittoendAnim(float time)
    {
        yield return new WaitForSeconds(time);
        alreadyAttacked = true;

    }
}
