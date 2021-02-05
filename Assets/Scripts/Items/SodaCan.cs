using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SodaCan : Item
{
    protected override void Awake()
    {
        name = "SodaCan";
        id = 23;
        normal = true;
    }
}
