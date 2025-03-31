using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarPlayer : MonoBehaviour
{
    public List<ItemType> hotbarlist;
    public int selecteditem;

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

        newselected();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && hotbarlist.Count > 0)
        {
            selecteditem = 0;
            newselected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && hotbarlist.Count > 0)
        {
            selecteditem = 1;
            newselected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && hotbarlist.Count > 0)
        {
            selecteditem = 2;
            newselected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && hotbarlist.Count > 0)
        {
            selecteditem = 3;
            newselected();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && hotbarlist.Count > 0)
        {
            selecteditem = 4;
            newselected();
        }
    }

    void newselected()
    {
        heart.SetActive(false);
        lung.SetActive(false);
        kidney.SetActive(false);
        liver.SetActive(false);
        stomach.SetActive(false);

        GameObject selectedItemgameobject = hotbar[hotbarlist[selecteditem]];
        selectedItemgameobject.SetActive(true);
    }

}
