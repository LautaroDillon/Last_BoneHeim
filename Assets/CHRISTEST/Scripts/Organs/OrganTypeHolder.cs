using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganTypeHolder : MonoBehaviour
{
    public string organName;
    public Sprite icon;
    [TextArea]
    public string description;
    public bool isEquipped;

    public ItemType type;
}
