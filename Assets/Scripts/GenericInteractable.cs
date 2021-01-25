using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericInteractable : MonoBehaviour {

    // To use this, attach this script to the interactable and subscribe relevant functions to this event
    public Action InteractEvent;
}
