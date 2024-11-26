using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingTrap : MonoBehaviour
{
    public float speed = 5f; // Speed at which the trap moves
    public Transform crushTarget; // Target position (where the trap will move toward)
    public float crushDistance = 2f; // The distance the trap will travel to crush the player
    public bool isActive = false; // Whether the trap is active
    public LayerMask playerLayer; // Layer of the player to detect

    private Vector3 initialPosition;
    private bool crushing = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isActive && !crushing)
        {
            // Move the trap towards the target position
            MoveTrap();
        }
    }

    void MoveTrap()
    {
        if (Vector3.Distance(transform.position, crushTarget.position) > crushDistance)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime); // Move trap downwards
        }
        else
        {
            crushing = true; // Stop moving when target is reached
        }
    }

    // Detect when player enters the crushing zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            CrushPlayer(other.gameObject);
        }
    }

    void CrushPlayer(GameObject player)
    {
        Debug.Log("Player Crushed!");
        player.GetComponent<PlayerHealth>().TakeDamage(100); // Deal damage to the player
        Destroy(player); // Example: Destroy the player when crushed
    }
}
