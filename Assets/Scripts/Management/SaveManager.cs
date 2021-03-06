using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager Singleton;

    private void Awake () {
        if (!Singleton) {
            Singleton = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (gameObject);
        }
    }

    private void Start () {
        // TODO: Proper saving and loading
        SaveLoad.NewGameNoSave ();
    }
}
