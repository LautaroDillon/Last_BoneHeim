using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    [Header("References")]
    public float moveSpeed = 3f;
    public float moveDistance = 5f;
    public Vector3 moveDirection = Vector3.forward;
    private Vector3 startPosition;
    [SerializeField] int sawDamage = 40;
    [SerializeField] float rotationSpeed = 360f;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float distanceMoved = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + moveDirection * distanceMoved;
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth damagableInterface = other.gameObject.GetComponent<PlayerHealth>();
        if (other.gameObject.tag == "Player" && damagableInterface != null)
        {
            Debug.Log("Hit Player!");
            damagableInterface.TakeDamage(sawDamage);
            Destroy(gameObject);
        }
    }
}
