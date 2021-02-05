using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ammo : Item
{
    public int bullets;

    protected override void Awake()
    {
        name = "Ammo";
        id = -1;
        normal = true;
        bullets = -1;
    }
}
