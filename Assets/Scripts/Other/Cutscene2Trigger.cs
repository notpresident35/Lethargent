using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene2Trigger : MonoBehaviour {

    [SerializeField] CutsceneManager manager;

    private void OnTriggerEnter (Collider other) {
        if (!manager.CutscenesHavePlayed [1]) {
            manager.StartCutscene (1);
            manager.CutscenesHavePlayed [1] = true;
        }
        Destroy (gameObject);
    }
}
