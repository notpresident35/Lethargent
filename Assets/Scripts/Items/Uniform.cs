using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Uniform : Item
{
    protected override void Awake()
    {
        name = "Uniform";
        id = 20;
        normal = true;
    }
}
