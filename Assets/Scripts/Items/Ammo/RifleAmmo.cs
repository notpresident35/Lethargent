using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RifleAmmo : Ammo
{
    protected override void Awake()
    {
        name = "Rifle Ammo";
        id = 9;
        normal = true;
        bullets = 20;
    }
}
