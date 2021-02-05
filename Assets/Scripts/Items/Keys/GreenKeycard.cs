using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GreenKeycard : Keycard
{
    protected override void Awake()
    {
        name = "Green Keycard";
        id = 16;
        normal = false;
        color = 3;
        door = "??";
    }

}
