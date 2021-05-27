using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : Item
{
    public int damage;

    protected override void Awake()
    {
        name = "Weapon";
        id = -1;
        normal = true;
        damage = -1;
    }

    public virtual int GetDamage()
    {
        return damage;
    }
}
