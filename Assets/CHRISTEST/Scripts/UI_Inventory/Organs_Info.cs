using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organs_Info : MonoBehaviour
{
    public GameObject heart_Info;
    public GameObject lungs_Info;
    public GameObject stomach_Info;

    public void _BTN_Heart()
    {
        heart_Info.SetActive(true);
        lungs_Info.SetActive(false);
        stomach_Info.SetActive(false);
    }
    public void _BTN_Lungs()
    {
        heart_Info.SetActive(false);
        lungs_Info.SetActive(true);
        stomach_Info.SetActive(false);
    }

    public void _BTN_Stomach()
    {
        heart_Info.SetActive(false);
        lungs_Info.SetActive(false);
        stomach_Info.SetActive(true);
    }

}
