using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public static Interact Instance;
    [SerializeField] public float maxDistance;
    [SerializeField] private Transform mainCamera;
    public bool interact;

    public void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray rayo = new Ray(mainCamera.position, mainCamera.forward);

            bool detectado = Physics.Raycast(rayo, out RaycastHit hit, maxDistance);
            if (!detectado) return;

            if (hit.collider.TryGetComponent(out Iteractuable iteractive))
            {
                if (!interact)
                {
                    iteractive.Activate();
                    interact = true;
                }
                else
                {
                    iteractive.Desactive();
                    interact = false;
                }
            }

        }
    }
}
