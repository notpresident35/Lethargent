using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
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
