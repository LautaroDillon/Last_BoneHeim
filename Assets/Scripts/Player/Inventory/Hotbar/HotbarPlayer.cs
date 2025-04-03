using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class HotbarPlayer : MonoBehaviour
{
    public List<ItemType> hotbarlist;
    public int selecteditem;

    public KeyCode useorgan = KeyCode.Q;

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
            Debug.Log("probando rueda");
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            selecteditem--;
            Debug.Log("probando rueda");
        }

        if (selecteditem <= 0)
        {
            selecteditem = 1;
        }
        else if (selecteditem >= 4)
        {
            selecteditem = 1;
        }
        if (Input.GetKeyDown(useorgan))
        {
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

        Debug.Log(selecteditem);
        GameObject selectedItemgameobject = hotbar[hotbarlist[selecteditem]];
        selectedItemgameobject.SetActive(true);
    }

}
