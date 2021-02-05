using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rifle : Weapon
{
    protected override void Awake()
    {
        name = "Rifle";
        id = 6;
        normal = true;
        damage = 20;
    }
}
