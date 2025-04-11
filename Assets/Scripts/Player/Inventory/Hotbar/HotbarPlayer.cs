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

    [SerializeField] GameObject lung;
    [SerializeField] GameObject kidney;
    [SerializeField] GameObject liver;
    [SerializeField] GameObject stomach;

    public DataOrgan[] dataOrgans;

    Dictionary<ItemType, GameObject> dataOrgansDict = new Dictionary<ItemType, GameObject>();

    private Dictionary<ItemType, GameObject> hotbar = new Dictionary<ItemType, GameObject>();

    void Start()
    {
        Instance = this;

        foreach (var item in dataOrgans)
        {
            if (!dataOrgansDict.ContainsKey(item.type))
                dataOrgansDict.Add(item.type, item.go);
        }
    }


    void Update()
    {
        var axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis < 0)
        {
            ScrollPos += speedscroll * Time.deltaTime;

            if (ScrollPos >= 1)
            {
                ScrollPos = 0;
                selecteditem++;
            }
        }
        else if(axis > 0)
        {
            ScrollNeg += speedscroll * Time.deltaTime;

            if (ScrollNeg >= 1)
            {
                ScrollNeg = 0;
                selecteditem--;
            }    
        }

        if (selecteditem <= 0)  
        {
            selecteditem = 3;
        }else if (selecteditem >= hotbar.Count && hotbar.Count + 1 > 0)
        {
            selecteditem = 0;
        }
        if (Input.GetKeyDown(selectorgan))
        {
            newselected();
        }
        if (Input.GetMouseButtonDown(1)/* && organselect*/ )
        {
            useselected();
        }
    }

    void newselected()
    {
        lung.SetActive(false);
        kidney.SetActive(false);
        liver.SetActive(false);
        stomach.SetActive(false);

        Debug.Log(selecteditem);
        GameObject selectedItemgameobject = hotbar[hotbarlist[selecteditem]];
        organselect = true; 
        selectedItemgameobject.SetActive(true);
    }


    void useselected()
    {
        foreach (KeyValuePair<ItemType, GameObject> item in hotbar)
        {
            Debug.Log($"Clave: {item.Key} - Valor: {item.Value}");
        }

        lung.SetActive(false);
        kidney.SetActive(false);
        liver.SetActive(false);
        stomach.SetActive(false);

        //hacer el llamdo para el funcionamiento de los organos
    }

    public void AddToHotbar(ItemType a)
    {
        Debug.Log("Adding to hotbar: " + a);
        if (!dataOrgansDict.ContainsKey(a)) return;

        //dataOrgansDict[a].SetActive(true);
        switch (a)
        {
            case ItemType.O_Lungs:
                hotbarlist.Add(a);
                hotbar.Add(a, lung);
                Debug.Log("Added Lungs");
                break;
            case ItemType.O_Kidney:
                hotbarlist.Add(a);
                hotbar.Add(a, kidney);
                Debug.Log("Added Kidney");
                break;
            case ItemType.O_Liver:
                hotbarlist.Add(a);
                hotbar.Add(a, liver);
                Debug.Log("Added Liver");
                break;
            case ItemType.O_Stomach:
                hotbarlist.Add(a);
                hotbar.Add(a, stomach);
                Debug.Log("Added Stomach");
                break;
        }

    }

    public void RemoveToHotbar(ItemType a)
    {
        Debug.Log("Adding to hotbar: " + a);
        switch (a)
        {
            case ItemType.O_Lungs:
                hotbar.Remove(a);
                Debug.Log("Added Lungs");
                break;
            case ItemType.O_Kidney:
                hotbar.Remove(a);
                Debug.Log("Added Kidney");
                break;
            case ItemType.O_Liver:
                hotbar.Remove(a);
                Debug.Log("Added Liver");
                break;
            case ItemType.O_Stomach:
                hotbar.Remove(a);
                Debug.Log("Added Stomach");
                break;
        }

    }
}

[System.Serializable]
public struct DataOrgan
{
    public ItemType type;
    public GameObject go;
}
