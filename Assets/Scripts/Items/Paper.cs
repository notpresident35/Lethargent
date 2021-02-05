using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paper : Item
{
    protected override void Awake()
    {
        name = "Paper";
        id = 22;
        normal = true;
    }
}
