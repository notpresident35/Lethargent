using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShotgunAmmo : Ammo
{
    protected override void Awake()
    {
        name = "Shotgun Ammo";
        id = 10;
        normal = true;
        bullets = 5;
    }
}
