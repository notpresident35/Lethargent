using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSetter : MonoBehaviour {
    
    public string baseLayerState;
    public string armLayerState; 

    Animator anim;

    private void Awake () {
        anim = GetComponent<Animator> ();
    }

    private void Start () {
        anim.Play (baseLayerState, 0);
        anim.Play (armLayerState, 2);
    }
}
