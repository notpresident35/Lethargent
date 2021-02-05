using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grenade : Weapon
{
    public int offDamage = 50;

    protected override void Awake()
    {
        name = "Grenade";
        id = 11;
        normal = true;
        damage = 100;
    }

    public override void PickUp()
    {
        LevelManager.current.playerData.UpdateItem(this, true);
        LevelManager.current.playerData.grenades += 1;
    }

    public override void Action()
    {
        LevelManager.current.playerData.grenades -= 1;
        if (LevelManager.current.playerData.medkits == 0)
        {
            LevelManager.current.playerData.UpdateItem(this, false);
        }
    }
}
