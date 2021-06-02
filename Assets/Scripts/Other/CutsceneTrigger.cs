using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour {

    public int ID;

    [SerializeField] CutsceneManager manager;

    private void OnTriggerEnter (Collider other) {
        //print (other.name);
        if (!LevelManager.current.completionStats.cutscenesWatched [ID]) {
            manager.StartCutscene (ID);
        }
        gameObject.SetActive (false);
    }
}
