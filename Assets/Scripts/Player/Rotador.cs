using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotador : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocidad de rotaci�n
    public float floatAmplitude = 0.5f; // Altura m�xima de subida y bajada
    public float floatSpeed = 2f; // Velocidad de movimiento vertical

    private Vector3 startPosition; // Posici�n inicial del objeto

    void Start()
    {
        // Guardar la posici�n inicial
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotar el objeto
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Subir y bajar suavemente
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
