using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupEventHandler : MonoBehaviour {

    public static ItemPickupEventHandler Instance;

    private void Awake () {
        Instance = this;
    }

    public void Pickup (int ID, Transform itemTransform) {
        if (ID == 22) {
            foreach (Transform paper in itemTransform.GetChild (0)) {
                paper.GetComponent<Outline> ().enabled = false;
            }
            itemTransform.GetChild (1).gameObject.SetActive (false);
        }
    }
}
