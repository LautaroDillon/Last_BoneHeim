using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class HotbarPlayer : MonoBehaviour
{
    public static HotbarPlayer Instance;

    float ScrollPos;
    float ScrollNeg;
    public float speedscroll;

    public List<ItemType> hotbarlist;
    public int selecteditem;
    public bool organselect;

    public KeyCode selectorgan = KeyCode.Q;

    public DataOrgan[] dataOrgans;

    public Dictionary<ItemType, GameObject> dataOrgansDict = new Dictionary<ItemType, GameObject>();

    public Transform throwSpawnPoint;
    public float throwForce = 20f;
    public PlayerGrenadeThrower playerGrenadeThrower;

    #region Unity Methods
    void Start()
    {
        Instance = this;
    }


    void Update()
    {
        if (Input.inputString != null)
        {
            bool isnumber = int.TryParse(Input.inputString, out int number);
            if (isnumber && number > 0 && number <= hotbarlist.Count)
            {
                Debug.Log("Selected item: " + number);
                selecteditem = number - 1; // Convert to zero-based index
                newselected();
            }
        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            selecteditem--;
            if (selecteditem < 0)
                selecteditem = hotbarlist.Count - 1;

            newselected();
        }
        else if (scroll < 0f)
        {
            selecteditem++;
            if (selecteditem >= hotbarlist.Count)
                selecteditem = 0;

            newselected();
        }


        if (Input.GetKeyDown(selectorgan) && organselect)
        {
            useselected();
        }

    }
    #endregion

    #region selection
    void newselected()
    {
        if (hotbarlist.Count == 0 || selecteditem < 0 || selecteditem >= hotbarlist.Count)
            return;

        ItemType currentType = hotbarlist[selecteditem];

        if (!dataOrgansDict.TryGetValue(currentType, out GameObject selectedOrgan))
        {
            Debug.LogWarning("Organ not found in dictionary: " + currentType);
            return;
        }

        // Activar solo el órgano seleccionado
        foreach (var organ in dataOrgans)
        {
            if (dataOrgansDict.TryGetValue(organ.type, out GameObject obj))
            {
                obj.SetActive(organ.type == currentType);

            }
        }

        organselect = true;
    }
    #endregion

    #region use selected organ
    void useselected()
    {
        if (hotbarlist.Count == 0 || selecteditem < 0 || selecteditem >= hotbarlist.Count)
            return;

        ItemType currentType = hotbarlist[selecteditem];

        string organName = currentType.ToString();

        foreach (var organ in dataOrgans)
        {
            if (dataOrgansDict.TryGetValue(organ.type, out GameObject obj))
            {
                obj.SetActive(false);
            }
        }

        PlayerUI.instance.isUsed(organName);
        EventAnim.instance.organDesactive(organName);
        //eleminar el organo de la lista de hotbar
        RemoveToHotbar(currentType);

        ThrowOrganAsGrenade(currentType);
        GrenadeType grenadeType = ConvertToGrenadeType(currentType);


        organselect = false;

        //hacer el llamdo para el funcionamiento de los organos
        switch (currentType)
        {
            case ItemType.O_Lungs:
                PlayerMovement.instance.canDash = false;
                break;
            case ItemType.O_Stomach:
                PlayerMovement.instance.canDoubleJump = false;
                break;
            case ItemType.O_Heart:
                break;

        }
    }

    GrenadeType ConvertToGrenadeType(ItemType item)
    {
        switch (item)
        {
            case ItemType.O_Lungs:
                return GrenadeType.Lung;
            case ItemType.O_Stomach:
                return GrenadeType.Stomach;
            case ItemType.O_Heart:
                return GrenadeType.Heart;
            default:
                Debug.LogWarning("ItemType no tiene conversión definida: " + item);
                return GrenadeType.Heart; // valor por defecto
        }
    }


    void ThrowOrganAsGrenade(ItemType itemType)
    {
        GrenadeType grenadeType = ConvertToGrenadeType(itemType);

        if (playerGrenadeThrower != null)
        {
            playerGrenadeThrower.ThrowGrenadeInstant(grenadeType, throwForce);
        }
    }

    #endregion

    #region adding/removing organs
    public void AddToHotbar(ItemType a)
    {
        Debug.Log("Adding to hotbar: " + a);
        hotbarlist.Add(a);

        //miro si el organo ya existe en el diccionario
        foreach (var item in dataOrgans)
        {
            //si el organo no existe en el diccionario, lo agrego
            if (item.type == a && !dataOrgansDict.ContainsKey(a))
            {
                dataOrgansDict.Add(a, item.objectType);
                break;
            }
        }

    }

    public void RemoveToHotbar(ItemType a)
    {
        Debug.Log("Removing to hotbar: " + a);
        hotbarlist.Remove(a);

        foreach (var item in dataOrgans)
        {
            if (dataOrgansDict.ContainsKey(a))
                dataOrgansDict.Remove(a);
            else
                Debug.Log("Item not exists in dictionary: " + item.type);
        }
    }
    #endregion
}

[System.Serializable]
public struct DataOrgan
{
    public ItemType type;
    public GameObject objectType;
}
