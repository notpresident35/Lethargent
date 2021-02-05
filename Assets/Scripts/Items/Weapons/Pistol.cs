using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pistol : Weapon
{
    protected override void Awake()
    {
        name = "Pistol";
        id = 5;
        normal = true;
        damage = 40;
    }
}
