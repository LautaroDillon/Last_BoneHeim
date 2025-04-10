using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Organ_Be : MonoBehaviour
{
    public static Organ_Be _organ_Be;

    public bool canbeused;
    public bool isused;
    protected ItemType whastisthis;
    protected GameObject esto;


    public enum type
    {
        organs,
        hands,
        arms
    }
}
