using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class HotbarPlayer : MonoBehaviour
{
    public List<ItemType> hotbarlist;
    public int selecteditem;
    public bool organselect;

    public KeyCode selectorgan = KeyCode.Q;

    [SerializeField] GameObject heart;
    [SerializeField] GameObject lung;
    [SerializeField] GameObject kidney;
    [SerializeField] GameObject liver;
    [SerializeField] GameObject stomach;

    private Dictionary<ItemType, GameObject> hotbar = new Dictionary<ItemType, GameObject>();

    void Start()
    {
        hotbar.Add(ItemType.O_Heart, heart);
        hotbar.Add(ItemType.O_Lungs, lung);
        hotbar.Add(ItemType.O_Kidney, kidney);
        hotbar.Add(ItemType.O_Liver, liver);
        hotbar.Add(ItemType.O_Stomach, stomach);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            selecteditem++;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selecteditem--;
        }

        if (selecteditem <= 0)
        {
            selecteditem = 1;
        }
        else if (selecteditem >= 4)
        {
            selecteditem = 1;
        }
        if (Input.GetKeyDown(selectorgan))
        {
            newselected();
        }
        if (Input.GetMouseButtonDown(1) && organselect)
        {
            useselected();
        }
    }

    void newselected()
    {
        heart.SetActive(false);
        lung.SetActive(false);
        kidney.SetActive(false);
        liver.SetActive(false);
        stomach.SetActive(false);

        Debug.Log(selecteditem);
        GameObject selectedItemgameobject = hotbar[hotbarlist[selecteditem]];
        selectedItemgameobject.SetActive(true);
    }


    void useselected()
    {
        heart.SetActive(false);
        lung.SetActive(false);
        kidney.SetActive(false);
        liver.SetActive(false);
        stomach.SetActive(false);

        //hacer el llamdo para el funcionamiento de los organos
    }
}
