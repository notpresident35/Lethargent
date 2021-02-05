using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInteractable : MonoBehaviour {
    // Make any iteractable object or item inherit from this script
    public virtual void Interact () {
        Debug.LogWarning ("Override this generic behavior!");
    }
}
