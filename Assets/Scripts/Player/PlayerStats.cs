using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    int m_currentItemID = -1;

    // Dictionary storing <Item ID: item quantity>
    // Collectibles dictionary with {item_id (String) : collected (Bool)}
    private Dictionary<int, int> m_itemsCollected = new Dictionary<int, int> ();

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
    public int currentItemID
    {
        get{return m_currentItemID;}
        set{ m_currentItemID = value;}
    }
    public Dictionary<int, int> itemsCollected
    {
       get{return m_itemsCollected;}
    }

    //Use bool norm to indicate whether to use normal items dict or key items dict
    public bool ItemRegistered(int id) {

        return m_itemsCollected.ContainsKey(id);
    }

    public void CollectItem (int id, int quantity) {
        if (m_itemsCollected.ContainsKey (id)) {
            m_itemsCollected [id] += quantity;
        } else {
            m_itemsCollected.Add (id, quantity);
        }
    }

    public void UpdateItem (int id, int quantity)
    {
        if(m_itemsCollected.ContainsKey(id)) {
            m_itemsCollected[id] = quantity;
            return;
        } else {
            Debug.LogWarning ("Don't use UpdateItem for an item the player hasn't collected yet without a good reason...");
            CollectItem (id, quantity);
        }
    }

    public void DropItem (int id, int quantity) {
        if (!m_itemsCollected.ContainsKey(id)) {
            Debug.LogError ("Tried to drop item the player doesn't have. What? Why?");
            return;
        }
        if (m_itemsCollected[id] > quantity) {
            m_itemsCollected [id] = m_itemsCollected [id] - quantity;
        } else {
            m_itemsCollected.Remove (id);
        }
    }

    // WARNING: This will remove all of the items with this id from the player!
    // Do not use this for items that have multiple copies or more than one use!
    public void RemoveItem (int id) {
        if (!m_itemsCollected.ContainsKey (id)) {
            Debug.LogError ("Tried to remove item the player doesn't have. What? Why?");
            return;
        }
        m_itemsCollected.Remove (id);
    }

    public int GetItemValue (int id)
    {
        if (m_itemsCollected.ContainsKey(id)) {
            return m_itemsCollected[id];
        }
        return 0;
    }

    public bool HasItem (int id) {
        return m_itemsCollected.ContainsKey (id);
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
