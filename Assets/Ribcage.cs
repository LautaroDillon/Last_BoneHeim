using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ribcage : MonoBehaviour
{
    public Transform player;
    public float activationDistance = 5f;

    private Animator animator;
    private bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (animator == null)
            Debug.LogError("No Animator found on ribcage.");
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldBeOpen = distance <= activationDistance;

        if (shouldBeOpen != isOpen)
        {
            isOpen = shouldBeOpen;
            animator.SetBool("isPlayerClose", isOpen);
        }
    }
}
