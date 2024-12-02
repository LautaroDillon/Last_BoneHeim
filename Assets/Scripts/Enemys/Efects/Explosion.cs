using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
   
    public float maxRadius;        
    public float expansionSpeed;    
    public float damageAmount;     
    public float duration;        

    private float currentRadius = 0;  
    private SphereCollider sphereCollider;
    private Transform visualSphere;
    [SerializeField] private AudioClip explosionClip;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 0f;

        // Crear una esfera visual que se expanda junto con el collider
        SoundManager.instance.PlaySound(explosionClip, transform, 1f, false);
        visualSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        visualSphere.SetParent(transform);
        visualSphere.localPosition = Vector3.zero;
        visualSphere.localScale = Vector3.zero;

        // Desactivar el collider de la esfera visual para que no interfiera
        Destroy(visualSphere.GetComponent<SphereCollider>());

        // Destruir el objeto después de la duración especificada
        Destroy(gameObject, duration);
    }

    void Update()
    {
        // Aumentar el tamaño del collider (onda expansiva)
        currentRadius += expansionSpeed * Time.deltaTime;
        sphereCollider.radius = currentRadius;

        // Aumentar el tamaño visual de la esfera para representar la expansión
        float visualScale = currentRadius * 2f;
        visualSphere.localScale = new Vector3(visualScale, visualScale, visualScale);

        // Si alcanza el radio máximo, deja de expandirse
        if (currentRadius >= maxRadius)
        {
            currentRadius = maxRadius;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
