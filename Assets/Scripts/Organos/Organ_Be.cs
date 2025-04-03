using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Organ_Be : MonoBehaviour
{
    public bool canbeused;
    public bool isused;

    public enum type
    {
        organs,
        hands,
        arms
    }

}
