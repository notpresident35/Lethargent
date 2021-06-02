using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : GenericInteractable {

    public float UniqueID;

    public int ItemQuantity = 1;
    public ItemData data;

    private void Awake () {
        UniqueID = transform.position.sqrMagnitude;
    }

    public virtual void PickUp()
    {
        AudioManager.Play2DSound (data.PickupSFX, Statics.SFXMixerGroupName, 1, false);
        LevelManager.current.playerData.CollectItem (data.ID, ItemQuantity);
        ItemPickupEventHandler.Instance.Pickup (data.ID, transform);
    }

    public virtual void SetHeld() {
        ItemPickupEventHandler.Instance.SetHeld (data.ID, transform);
    }

    public virtual void SetUnHeld () {
        ItemPickupEventHandler.Instance.SetUnHeld (data.ID, transform);
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

    private void OnEnable () {
        SaveLoad.SyncDataForSave += SyncDataForSave;
        SaveLoad.SyncDataOnLoad += SyncDataOnLoad;
    }

    private void OnDisable () {
        SaveLoad.SyncDataForSave -= SyncDataForSave;
        SaveLoad.SyncDataOnLoad -= SyncDataOnLoad;
    }

    void SyncDataForSave () {
        if (LevelManager.current.playerData.itemsPositions.ContainsKey (UniqueID)) {
            LevelManager.current.playerData.itemsPositions [UniqueID] = transform.position;
            LevelManager.current.playerData.itemsRotations [UniqueID] = transform.rotation;
        } else {
            LevelManager.current.playerData.itemsPositions.Add (UniqueID, transform.position);
            LevelManager.current.playerData.itemsRotations.Add (UniqueID, transform.rotation);
        }
    }

    void SyncDataOnLoad () {
        if (LevelManager.current.playerData.itemsPositions.ContainsKey (UniqueID)) {
            transform.position = LevelManager.current.playerData.itemsPositions [UniqueID];
            transform.rotation = LevelManager.current.playerData.itemsRotations [UniqueID];
        }
    }
}
