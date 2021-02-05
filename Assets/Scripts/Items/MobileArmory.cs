using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MobileArmory : Item
{
    protected override void Awake()
    {
        name = "Mobile Armory";
        id = 12;
        normal = true;
    }
}
