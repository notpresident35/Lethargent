using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Keycard : Key
{
    public enum Color
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple
    }
    public int color;

    protected override void Awake()
    {
        name = "Keycard";
        id = -1;
        normal = false;
        door = "";
        color = -1;
    }
}
