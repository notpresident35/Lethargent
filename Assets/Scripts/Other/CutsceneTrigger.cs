using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour {

    public int ID;

    [SerializeField] CutsceneManager manager;

    private void OnTriggerEnter (Collider other) {
        //print (other.name);
        if (!manager.CutscenesHavePlayed [ID]) {
            manager.StartCutscene (ID);
            manager.CutscenesHavePlayed [ID] = true;
        }
        Destroy (gameObject);
    }
}
