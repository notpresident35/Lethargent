using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats
{
    //Default position of player in scene
    float m_playerPosX = 0f;
    float m_playerPosY = 0f;
    float m_playerPosZ = 0f;

    int m_sceneID = 0; //Default scene

    bool m_finishedGame = false;

    // Collectibles dictionary with {item_id (String) : collected (Bool)}
    private Dictionary<Item, bool> m_normItems = new Dictionary<Item, bool>();

    private Dictionary<Item, bool> m_keyItems = new Dictionary<Item, bool>();

    public float playerPosX
    {
      get{return m_playerPosX;}
      set{m_playerPosX = value;}
    }
    public float playerPosY
    {
      get{return m_playerPosY;}
      set{m_playerPosY = value;}
    }
    public float playerPosZ
    {
      get{return m_playerPosZ;}
      set{m_playerPosZ = value;}
    }
    public int sceneID
    {
      get{return m_sceneID;}
      set{m_sceneID = value;}
    }
    public bool finishedGame
    {
      get{return m_finishedGame;}
      set{m_finishedGame = value;}
    }
    public Dictionary<Item, bool> normItems
    {
       get{return m_normItems;}
    }
    public Dictionary<Item, bool> keyItems
    {
       get{return m_keyItems;}
    }

    //Use bool norm to indicate whether to use normal items dict or key items dict
    public bool ItemRegistered(Item id)
    {
        if(m_normItems.ContainsKey(id))
        {
            return true;
        }
        return m_keyItems.ContainsKey(id);
    }

    public void AddItem(Item id, bool norm, bool value=false)
    {
        if(norm)
        {
            m_normItems.Add(id, value);
            return;
        }
        m_keyItems.Add(id, value);
    }

    public void UpdateItem(Item id, bool new_value)
    {
        if(m_normItems.ContainsKey(id))
        {
            m_normItems[id] = new_value;
            return;
        }
        m_keyItems[id] = new_value;
    }

    public bool GetItem(Item id)
    {
        if(m_normItems.ContainsKey(id))
        {
            return m_normItems[id];
        }
        return m_keyItems[id];
    }
}
