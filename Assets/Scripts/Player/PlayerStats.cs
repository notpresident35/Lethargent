using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerStats
{
    int m_health = 100;
    int m_grenades = 0;
    int m_medkits = 0;

    //Default position of player in scene
    float m_playerPosX = 0f;
    float m_playerPosY = 0f;
    float m_playerPosZ = 0f;

    int m_sceneID = 0; //Default scene

    bool m_finishedGame = false;

    Weapon m_currentWeapon = null;

    // Collectibles dictionary with {item_id (String) : collected (Bool)}
    private Dictionary<Item, bool> m_normItems = new Dictionary<Item, bool>()
    {
        {new Pistol(), false}, {new Rifle(), false}, {new Shotgun(), false},
        {new PistolAmmo(), false}, {new RifleAmmo(), false}, {new ShotgunAmmo(), false},
        {new Grenade(), false}, {new MobileArmory(), false}, {new Medkit(), false},
        {new Uniform(), false}, {new Orange(), false}, {new Paper(), false},
        {new SodaCan(), false}, {new Stapler(), false}
    };

    private Dictionary<Item, bool> m_keyItems = new Dictionary<Item, bool>()
    {
      {new MetalKey(), false}, {new BarnKey(), false}, {new BlackKey(), false},
      {new LabKey(), false}, {new RedKeycard(), false}, {new BlueKeycard(), false},
      {new GreenKeycard(), false}, {new YellowKeycard(), false}, {new PurpleKeycard(), false}
    };

    public int health
    {
      get{return m_health;}
      set{m_health = value;}
    }
    public int grenades
    {
      get{return m_grenades;}
      set{m_grenades = value;}
    }
    public int medkits
    {
      get{return m_medkits;}
      set{m_medkits = value;}
    }
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
    public Weapon currentWeapon
    {
        get{return m_currentWeapon;}
        set{m_currentWeapon = value;}
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

    public PlayerStats () {
        /*
        sceneID = 0;
        playerPosX = 0;
        playerPosX = 0;
        playerPosX = 0;
        */
    }
}
