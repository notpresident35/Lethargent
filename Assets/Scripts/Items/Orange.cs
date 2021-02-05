using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Orange : Item
{
    protected override void Awake()
    {
        name = "Orange";
        id = 21;
        normal = true;
    }
}
