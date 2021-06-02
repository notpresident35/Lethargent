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
    Vector3 m_playerPos = Vector3.zero;
    Quaternion m_playerRot = Quaternion.identity;
    Vector3 m_cameraPos = Vector3.zero;
    Quaternion m_cameraRot = Quaternion.identity;

    int m_sceneID = 0; //Default scene

    bool m_finishedGame = false;

    float m_heldItemUniqueID = -1;
    int m_currentItemID = -1;

    // Dictionary storing <Item ID: item quantity>
    // Collectibles dictionary with {item_id (String) : collected (Bool)}
    private Dictionary<int, int> m_itemsCollected = new Dictionary<int, int> ();
    private Dictionary<float, bool> m_itemsHeld = new Dictionary<float, bool> ();

    private Dictionary<float, Vector3> m_itemsPositions = new Dictionary<float, Vector3> ();
    private Dictionary<float, Quaternion> m_itemsRotations = new Dictionary<float, Quaternion> ();

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
    public Vector3 playerPos
    {
      get{return m_playerPos;}
      set{m_playerPos = value;}
    }
    public Quaternion playerRot {
        get { return m_playerRot; }
        set { m_playerRot = value; }
    }
    public Vector3 cameraPos {
        get { return m_cameraPos; }
        set { m_cameraPos = value; }
    }
    public Quaternion cameraRot {
        get { return m_cameraRot; }
        set { m_cameraRot = value; }
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
    public float heldItemUniqueID {
        get { return m_heldItemUniqueID; }
        set { m_heldItemUniqueID = value; }
    }
    public Dictionary<int, int> itemsCollected
    {
       get{return m_itemsCollected;}
    }
    public Dictionary<float, bool> itemsHeld {
        get { return m_itemsHeld; }
    }
    public Dictionary<float, Vector3> itemsPositions {
        get { return m_itemsPositions; }
    }
    public Dictionary<float, Quaternion> itemsRotations {
        get { return m_itemsRotations; }
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
