using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewItem", menuName = "ScriptableObjects/SimpleItemData")]
[System.Serializable]
public class ItemData : ScriptableObject {

    [System.Serializable]
    public enum Type {
        Default,
        Weapon,
        Consumable,
        KeyItem,
        Unusable
    }

    public AudioClip PickupSFX;
    public string itemName;
    public int ID;
    public int quantity = 1;
    public bool usable;
    public bool oneHanded = false;
    public Type type;
}
