using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class OrganSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private ItemType organType;

    public void SetOrgan(ItemType type)
    {
        organType = type;
        nameText.text = type.ToString();
        // iconImage.sprite = tu sprite si tienes uno por tipo
    }

    public void OnRemoveClicked()
    {
        HotbarPlayer.Instance.RemoveToHotbar(organType);
        OrganInventoryUI.Instance.RefreshInventory(HotbarPlayer.Instance.hotbarlist);
    }
}
