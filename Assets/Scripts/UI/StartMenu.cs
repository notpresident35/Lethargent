using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    bool loading = false;

    // TODO: Replace with buttons
    private void Update () {
        if (Input.anyKeyDown && !loading) {
            loading = true;
            NewGame ();
        }
    }

    // TODO: Override save data with a fresh start
    public void NewGame () {
        SceneManager.LoadScene ("Main");
    }

    // TODO: Load saved game
    public void LoadGame () {
        SceneManager.LoadScene ("Main");
    }

    public void Quit () {
        Application.Quit ();
    }
}
