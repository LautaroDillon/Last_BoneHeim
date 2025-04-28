using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask interactableLayers;

    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TextMeshProUGUI promptText;

    private Camera _cam;
    private IInteractable _currentTarget;

    void Awake()
    {
        _cam = Camera.main;
        promptUI.SetActive(false);
    }

    void Update()
    {
        CheckForInteractable();
        HandleInput();
    }

    private void CheckForInteractable()
    {
        Ray ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, interactableLayers))
        {
            if (hit.collider.CompareTag("Organ"))
            {
                promptText.text = "Press E to pick up organ";
                promptUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                    hit.collider.GetComponent<PlayerOrgans>()?.Interact();
                return;
            }
        }

        promptUI.SetActive(false);
    }

    private void HandleInput()
    {
        if (_currentTarget != null && Input.GetKeyDown(KeyCode.X))
        {
            _currentTarget.Interact();
        }
    }
}
