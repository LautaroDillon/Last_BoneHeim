using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Illusion : MonoBehaviour
{
    private float lifetime;
    private Transform player;
    private NavMeshAgent navMeshAgent;

    public void Initialize(float duration, Transform playerTarget)
    {
        lifetime = duration;
        player = playerTarget;
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent != null && player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }

        StartCoroutine(DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (navMeshAgent != null && player != null)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
