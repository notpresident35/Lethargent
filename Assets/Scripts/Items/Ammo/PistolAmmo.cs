using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PistolAmmo : Ammo
{
    protected override void Awake()
    {
        name = "Pistol Ammo";
        id = 8;
        normal = true;
        bullets = 8;
    }
}
