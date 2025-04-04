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
    float axisScrollPos;
    float axisScrollNeg;
    public float speed;
    void Update()
    {
        axisScrollPos -= Time.deltaTime;
        if (axisScrollPos < 0) axisScrollPos = 0;
        axisScrollNeg -= Time.deltaTime;
        if (axisScrollNeg < 0) axisScrollNeg = 0;

        var axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis < 0)
        {
            axisScrollPos += speed * Time.deltaTime;

            axisScrollNeg = 0;
            if (axisScrollPos >= 1)
            {
                axisScrollPos = 0;
                selecteditem++;
            }
        }
        else if(axis > 0)
        {
            axisScrollNeg += speed * Time.deltaTime;

            axisScrollPos = 0;
            if (axisScrollNeg >= 1)
            {
                axisScrollNeg = 0;
                selecteditem--;
            }
           
        }

        if (selecteditem <= 0)
        {
            selecteditem = 3;
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
