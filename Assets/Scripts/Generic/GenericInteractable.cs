using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericInteractable : MonoBehaviour
{
    // Make any iteractable object or item inherit from this script
    public virtual void InteractEvent() {}
}
