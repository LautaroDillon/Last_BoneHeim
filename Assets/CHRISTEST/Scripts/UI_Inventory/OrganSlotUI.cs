using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrganSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    private ItemType organType;

    public void SetOrgan(ItemType type)
    {
        organType = type;

        foreach (var organ in HotbarPlayer.Instance.dataOrgans)
        {
            if (organ.type == type)
            {
                iconImage.sprite = organ.icon;
                nameText.text = organ.organName;
                break;
            }
        }
    }

    public void BTN_show()
    {
        OrganInventoryUI.Instance.ShowDescription(organType);

    }

    // Cuando el mouse entra en el slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        OrganInventoryUI.Instance.ShowDescription(organType);
    }

    // Cuando el mouse sale
    public void OnPointerExit(PointerEventData eventData)
    {
        OrganInventoryUI.Instance.hide();
    }
}
