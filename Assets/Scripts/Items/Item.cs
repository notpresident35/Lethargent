using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : GenericInteractable {

    public int ItemQuantity = 1;
    public ItemData data;

    public virtual void PickUp()
    {
        AudioManager.Play2DSound (data.PickupSFX, Statics.SFXMixerGroupName, 1, false);
        LevelManager.current.playerData.CollectItem (data.ID, ItemQuantity);
        ItemPickupEventHandler.Instance.Pickup (data.ID, transform);
    }

    public virtual void Drop()
    {
        if (data.usable)
        {
            LevelManager.current.playerData.DropItem (data.ID, ItemQuantity);
        }
    }

    public virtual void Use()
    {
        if (data.usable)
        {
            Action();
        }
    }

    public virtual void Action() {}

    public override void Interact () {
        PickUp ();
    }
}
