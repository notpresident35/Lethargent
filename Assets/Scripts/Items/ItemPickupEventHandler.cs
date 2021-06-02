using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupEventHandler : MonoBehaviour {

    public static ItemPickupEventHandler Instance;

    public GameObject Gateway0;

    private void Awake () {
        Instance = this;
    }

    public void Pickup (int ID, Transform itemTransform) {
        if (ID == 22) {
            foreach (Transform paper in itemTransform.GetChild (0)) {
                paper.GetComponent<Outline> ().enabled = false;
            }
            itemTransform.GetChild (1).gameObject.SetActive (false);
            Gateway0.SetActive (false);
        }
    }


    // Used to set the state of certain things when a player is holding an item in a loaded save
    // Separate from pickup because certain events should only occur when the item is first picked up,
    // not every time the save is loaded.
    public void SetHeld (int ID, Transform itemTransform) {
        if (ID == 22) {
            foreach (Transform paper in itemTransform.GetChild (0)) {
                paper.GetComponent<Outline> ().enabled = false;
            }
            itemTransform.GetChild (1).gameObject.SetActive (false);
            Gateway0.SetActive (false);
        }
    }

    // Used to set the state of certain things when a player is NOT holding an item in a loaded save
    // When the game is loaded, the main scene is not reloaded because it's unnecessary and breaks things anyway
    // Thus, the item must reset its state manually when it was previously held but now is not because a save was loaded
    // Note that this is not necessarily the reverse process of SetHeld. For example, an item might open gateways in the early game,
    // so those gateways would need to be closed if the item isn't held, but those gateways should be open if the player is well past the tutorial,
    // even if the item is not held
    public void SetUnHeld (int ID, Transform itemTransform) {
        if (ID == 22) {
            foreach (Transform paper in itemTransform.GetChild (0)) {
                paper.GetComponent<Outline> ().enabled = true;
            }
            itemTransform.GetChild (1).gameObject.SetActive (true);
            Gateway0.SetActive (!LevelManager.current.completionStats.cutscenesWatched[1]);
        }
    }
}
