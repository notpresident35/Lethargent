using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shotgun : Weapon
{
    protected override void Awake()
    {
        name = "Shotgun";
        id = 7;
        normal = true;
        damage = 20;
    }
}
