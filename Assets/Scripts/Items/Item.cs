using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    private string name;
    private int id;
    private bool normal;

    public Item(string _name, int _id, bool _norm)
    {
        name = _name;
        id = _id;
        normal = _norm;
    }

    public virtual void PickUp()
    {
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
}
