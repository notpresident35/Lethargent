using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Medkit : Item
{
    protected override void Awake()
    {
        name = "Medkit";
        id = 19;
        normal = true;
    }

    public override void PickUp()
    {
        LevelManager.current.playerData.UpdateItem(this, true);
        LevelManager.current.playerData.medkits += 1;
    }

    public override void Action()
    {
        LevelManager.current.playerData.health = 100;
        LevelManager.current.playerData.medkits -= 1;
        if (LevelManager.current.playerData.medkits == 0)
        {
            LevelManager.current.playerData.UpdateItem(this, false);
        }
    }
}
