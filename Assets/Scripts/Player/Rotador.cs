using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotador : MonoBehaviour
{
    public float rotationSpeedX = 0f; // Velocidad de rotaci�n x
    public float rotationSpeedY = 0f; // Velocidad de rotaci�n y 
    public float rotationSpeedZ= 0f; // Velocidad de rotaci�n z
    public float floatAmplitude = 0.5f; // Altura m�xima de subida y bajada
    public float floatSpeed = 2f; // Velocidad de movimiento vertical

    private Vector3 startPosition; // Posici�n inicial del objeto

    void Start()
    {
        // Guardar la posici�n inicial
        startPosition = transform.position;
        rotationSpeedY = Random.Range(-40, 40);
    }

    void Update()
    {
        // Rotar el objeto
        transform.Rotate(rotationSpeedX * Time.deltaTime, rotationSpeedY * Time.deltaTime, rotationSpeedZ * Time.deltaTime);

        // Subir y bajar suavemente
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
