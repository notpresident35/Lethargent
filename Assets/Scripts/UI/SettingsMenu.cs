using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour {

    public GameObject PauseMenu, OptionsMenu;

    public GameObject PauseMenuAutoSelection, OptionsMenuAutoSelection;

    [SerializeField] bool startMenu = false;

    bool optionsOpen = false;
    InputManager control;

    private void Awake () {
        PauseMenu.SetActive (false);
        OptionsMenu.SetActive (false);
        control = FindObjectOfType <InputManager> ();
    }

    private void Start () {
        if (startMenu) {
            Statics.GameIsPaused = true;
            PauseMenu.SetActive (true);
            EventSystem.current.SetSelectedGameObject (PauseMenuAutoSelection);
        }
    }

    private void Update () {
        if (control.pause) {
            //print ("Tried to pause");
            if (Statics.GameIsPaused && optionsOpen) {
                ToggleOptions ();
            } else {
                TogglePause ();
            }
        }
        if (!startMenu) {
            UpdateCursor ();
        }
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
        if (!startMenu) {
            PauseMenu.SetActive (Statics.GameIsPaused);
        }
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

    public void NewGame () {
        SceneManager.LoadScene ("Main");
        Statics.GameIsPaused = false;
        PauseMenu.SetActive (false);
        gameObject.SetActive (false);
        Destroy (gameObject);
    }

    public void LoadGame () {
        SceneManager.LoadScene ("Main");
        Statics.GameIsPaused = false;
        PauseMenu.SetActive (false);
        gameObject.SetActive (false);
        Destroy (gameObject);
    }
}
