using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController Singleton;

    private void Awake () {
        if (Singleton) {
            gameObject.SetActive (false);
            Destroy (gameObject);
            return;
        } else {
            Singleton = this;
            DontDestroyOnLoad (gameObject);
        }
    }
}
