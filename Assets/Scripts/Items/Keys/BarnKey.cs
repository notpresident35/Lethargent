using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BarnKey : Key
{
    protected override void Awake()
    {
        name = "Barn Key";
        id = 2;
        normal = false;
        door = "Barn";
    }
}
