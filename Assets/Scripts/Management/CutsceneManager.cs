using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CutsceneManager : MonoBehaviour {

    public static bool Active;
    public static bool CanContinue;
    public static int CutsceneID;

    private void Update () {
        if (Input.GetKeyDown (KeyCode.Space) && Active && CanContinue) {
            Continue ();
        }
    }

    public void Continue () {
        CanContinue = false;
    }

    public void SetContinueFlag () {
        CanContinue = true;
    }
}
