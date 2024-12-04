using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    public ENecro necro;

    public float rotationSpeed = 100f;
    public float lifetime = 5f;     

    public float timer;

    void Start()
    {
        necro.canability = false;
        timer = lifetime;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        timer -= Time.deltaTime;

        
        if (timer <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
