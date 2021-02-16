using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecticalBox : GenericInteractable {

    public override void Interact () {
        Debug.Log (gameObject.name + " has been interacted with!");
    }
}
