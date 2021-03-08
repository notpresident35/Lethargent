using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour {

    public GameObject PauseMenu, OptionsMenu;

    public GameObject PauseMenuAutoSelection, OptionsMenuAutoSelection;

    bool optionsOpen = false;
    PlayerControlMapping control;

    private void Awake () {
        PauseMenu.SetActive (false);
        OptionsMenu.SetActive (false);
        control = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerControlMapping> ();
    }

    private void Update () {
        if (control.pause) {
            if (Statics.GameIsPaused && optionsOpen) {
                ToggleOptions ();
            } else {
                TogglePause ();
            }
        }
        UpdateCursor ();
    }

    void UpdateCursor () {
        if (Statics.GameIsPaused) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else if (!CutsceneManager.Active && (CameraScript.mouseReleased || control.aiming)) {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void TogglePause () {
        Statics.GameIsPaused = !Statics.GameIsPaused;
        PauseMenu.SetActive (Statics.GameIsPaused);
        Time.timeScale = Statics.GameIsPaused ? 0 : 1;
        if (Statics.GameIsPaused) {
            EventSystem.current.SetSelectedGameObject (PauseMenuAutoSelection);
        }
    }

    public void ToggleOptions () {
        optionsOpen = !optionsOpen;
        PauseMenu.SetActive (!optionsOpen);
        OptionsMenu.SetActive (optionsOpen);
        if (optionsOpen) {
            EventSystem.current.SetSelectedGameObject (OptionsMenuAutoSelection);
        } else {
            EventSystem.current.SetSelectedGameObject (PauseMenuAutoSelection);
        }
    }

    public void Quit () {
        Application.Quit ();
    }
}
