using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Key : Item
{
    public string door;

    protected override void Awake()
    {
        name = "Key";
        id = -1;
        normal = false;
        door = "";
    }
}
