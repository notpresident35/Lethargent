using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlackKey : Key
{
    protected override void Awake()
    {
        name = "Black Key";
        id = 3;
        normal = false;
        door = "??";
    }

}
