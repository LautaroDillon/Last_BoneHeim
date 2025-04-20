using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCooling : MonoBehaviour
{
    private Material material;
    public Transform[] waypoints; // Lista de waypoints
    private int currentWaypointIndex = 0;
    private float emissionStrength = 0f; // Inicia fr�o
    public float transitionSpeed = 2.0f; // Velocidad de cambio de emisi�n

    void Start()
    {
        material = GetComponent<Renderer>().material;

        if (!material.HasProperty("_Emission"))
        {
            Debug.LogWarning("El shader no tiene la propiedad _Emission");
            return;
        }

        material.EnableKeyword("_EMISSION"); // Activa la emisi�n en el material
    }

    void Update()
    {
        if (material == null || waypoints.Length == 0) return;

        // Obtener el waypoint actual y el siguiente
        Transform currentWaypoint = waypoints[currentWaypointIndex];
        Transform nextWaypoint = waypoints[(currentWaypointIndex + 1) % waypoints.Length];

        // Distancia entre la lava y los waypoints
        float distanceToCurrent = Mathf.Abs(transform.position.y - currentWaypoint.position.y);
        float distanceToNext = Mathf.Abs(transform.position.y - nextWaypoint.position.y);

        // Si est� MUY cerca de un waypoint, forzamos emisi�n a 1 o 0
        if (distanceToNext < 0.1f)
            emissionStrength = Mathf.MoveTowards(emissionStrength, 1f, Time.deltaTime * transitionSpeed);
        else if (distanceToCurrent < 0.1f)
            emissionStrength = Mathf.MoveTowards(emissionStrength, 0f, Time.deltaTime * transitionSpeed);
        else
        {
            // Interpolamos suavemente entre caliente (1) y fr�o (0)
            float t = Mathf.InverseLerp(0, distanceToCurrent + distanceToNext, distanceToNext);
            emissionStrength = Mathf.Lerp(1, 0, t);
        }

        // Aplicamos el valor de emisi�n al shader
        material.SetFloat("_Emission", emissionStrength);

        // Si llega al waypoint, cambia al siguiente
        if (distanceToNext < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
