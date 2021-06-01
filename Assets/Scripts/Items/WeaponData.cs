using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "NewItem", menuName = "ScriptableObjects/WeaponData")]
[System.Serializable]
public class WeaponData : ItemData {

    public int damage;
    public int fireRate;
    public GameObject projectile;
}
