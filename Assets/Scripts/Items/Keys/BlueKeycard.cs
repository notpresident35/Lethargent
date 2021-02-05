using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlueKeycard : Keycard
{
    protected override void Awake()
    {
        name = "Blue Keycard";
        id = 15;
        normal = false;
        color = 2;
        door = "??";
    }

}
