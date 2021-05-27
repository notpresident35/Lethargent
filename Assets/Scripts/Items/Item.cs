﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : GenericInteractable
{
    public AudioClip PickupSFX;
    protected string name;
    protected int id;
    protected bool normal;

    protected virtual void Awake()
    {
        name = "";
        id = -1;
        normal = false;
    }

    public virtual void PickUp()
    {
        AudioManager.Play2DSound (PickupSFX, Statics.SFXMixerGroupName, 1, false);
        LevelManager.current.playerData.UpdateItem(this, true);
    }

    public virtual void Drop()
    {
        if (normal)
        {
            LevelManager.current.playerData.UpdateItem(this, false);
        }
    }

    public virtual void Use()
    {
        if (normal)
        {
            Action();
        }
    }

    public virtual void Action() {}

    public override void Interact () {
        PickUp ();
    }
}
