using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PurpleKeycard : Keycard
{
    protected override void Awake()
    {
        name = "Purple Keycard";
        id = 18;
        normal = false;
        color = 5;
        door = "??";
    }

}
