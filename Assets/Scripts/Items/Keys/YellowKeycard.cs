using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class YellowKeycard : Keycard
{
    protected override void Awake()
    {
        name = "Yellow Keycard";
        id = 17;
        normal = false;
        color = 4;
        door = "??";
    }

}
