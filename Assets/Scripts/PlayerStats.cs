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

    // Collectibles dictionary with {collectible_id (String) : collected (Bool)}
    private Dictionary<string, bool> m_collectibles = new Dictionary<string, bool>();

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
    public Dictionary<string, bool> collectibles
    {
       get{return m_collectibles;}
    }

    public bool collectibleRegistered(string id)
    {
        return m_collectibles.ContainsKey(id);
    }

    public void addCollectible(string id, bool value=false)
    {
        m_collectibles.Add(id, value);
    }

    public void updateCollectibles(string id, bool new_value)
    {
        m_collectibles[id] = new_value;
    }

    public bool getCollectibleItem(string id)
    {
        return m_collectibles[id];
    }
}
