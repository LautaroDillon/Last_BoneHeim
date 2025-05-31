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

    // private Dictionary<ItemType, GameObject> hotbar = new Dictionary<ItemType, GameObject>();

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


        /*var axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis < 0)
        {
            ScrollPos += speedscroll * Time.deltaTime;
            organselect = !organselect;
            newselected();

            if (ScrollPos >= 1)
            {
                ScrollPos = 0;
                selecteditem++;
            }
        }
        else if(axis > 0)
        {
            ScrollNeg += speedscroll * Time.deltaTime;
            organselect = !organselect;
            newselected();

            if (ScrollNeg >= 1)
            {
                ScrollNeg = 0;
                selecteditem--;
            }    
        }

        if (selecteditem <= 0.5f)  
        {
            selecteditem = dataOrgansDict.Count;
        }
        else if (selecteditem >= dataOrgansDict.Count && dataOrgansDict.Count + 1 > 0)
        {
            selecteditem = 0;
        }*/

        if (Input.GetKeyDown(selectorgan) && organselect)
        {
            useselected();
        }

    }

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


    void useselected()
    {
        if (hotbarlist.Count == 0 || selecteditem < 0 || selecteditem >= hotbarlist.Count)
            return;

        ItemType currentType = hotbarlist[selecteditem];

        // desactivá el GameObject si existe
        if (dataOrgansDict.TryGetValue(currentType, out GameObject organGO))
        {
            organGO.SetActive(false);
        }
        else
        {
            Debug.LogWarning("El órgano seleccionado no está en el diccionario: " + currentType);
        }

        //eleminar el organo de la lista de hotbar
        RemoveToHotbar(currentType);

        organselect = false;

        //hacer el llamdo para el funcionamiento de los organos
    }

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
}

[System.Serializable]
public struct DataOrgan
{
    public ItemType type;
    public GameObject objectType;
}
