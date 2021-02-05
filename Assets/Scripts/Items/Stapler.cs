using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stapler : Item
{
    protected override void Awake()
    {
        name = "Stapler";
        id = 24;
        normal = true;
    }
}
