using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RedKeycard : Keycard
{
    protected override void Awake()
    {
        name = "Red Keycard";
        id = 14;
        normal = false;
        color = 1;
        door = "??";
    }

}
