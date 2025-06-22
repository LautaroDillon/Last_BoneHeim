using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrganInventoryUI : MonoBehaviour
{
    public static OrganInventoryUI Instance;

    [Header("Inventory Panel")]
    public GameObject inventoryPanel; // El contenedor principal (Panel)

    [Header("Slots")]
    public Transform contentParent;   // Content del ScrollView
    public GameObject slotPrefab;     // Prefab del Slot de órgano

    public TextMeshProUGUI descriptionText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Toggle con Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    /// <summary>
    /// Refresca la lista visual de órganos.
    /// </summary>
    public void RefreshInventory(List<ItemType> hotbarList)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemType organ in hotbarList)
        {
            GameObject slot = Instantiate(slotPrefab, contentParent);
            OrganSlotUI slotUI = slot.GetComponent<OrganSlotUI>();
            /*if (slotUI != null)
            {
                slotUI.SetOrgan(organ);
            }*/

            if (slotUI == null)
            {
                Debug.LogError("El prefab Organ_Slot NO tiene el script OrganSlotUI!");
            }
            else
            {
                slotUI.SetOrgan(organ);
            }
        }
    }

    public void ShowDescription(ItemType organType)
    {
        foreach (var organ in HotbarPlayer.Instance.dataOrgans)
        {
            if (organ.type == organType)
            {
                descriptionText.text = organ.description;
                break;
            }
        }
    }

    public void hide()
    {
        descriptionText.text = "";
    }
}
