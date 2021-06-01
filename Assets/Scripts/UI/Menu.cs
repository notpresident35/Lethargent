using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public static event Action GameStart;

    public GameObject PrimaryMenuAutoSelection, OptionsMenuAutoSelection;
    public GameObject PrimaryMenu;
    public GameObject OptionsMenu;
    public Slider volumeSlider;
    public Slider resolutionSlider;
    public Text currentResDisplay;
    public GameObject SaveButton;
    public Sprite ContinueButtonAltTextSprite;
    public Image ContinueButtonImage;
    public float ContinueButtonAltYPosition;

    int resolutionIndex = 0;
    float volume = 0.8f;
    bool optionsMenuOpen = false;
    InputManager control;
    Resolution [] resolutions; // List of all compatible resolutions

    private void Awake () {
        PrimaryMenu.SetActive (false);
        OptionsMenu.SetActive (false);
        control = FindObjectOfType<InputManager> ();
    }

    private void Start () {
        // Finds the index of the current screen resolution and sets the slider's value to match it
        resolutions = Screen.resolutions;
        resolutionSlider.maxValue = resolutions.Length - 1;
        resolutionIndex = -1;

        for (int i = 0; i < resolutions.Length; i++) {
            if (resolutions [i].Equals (Screen.currentResolution)) {
                resolutionIndex = i;
                break;
            }
        }

        if (resolutionIndex > 0) {
            resolutionSlider.value = resolutionIndex;
            UpdateResolution (resolutionIndex);
        } else {
            Debug.LogError ("Current screen resolution not found in list of valid resolutions! How?!");
        }

        Statics.GameIsPaused = true;
        PrimaryMenu.SetActive (true);
        OptionsMenu.SetActive (false);
        EventSystem.current.SetSelectedGameObject (PrimaryMenuAutoSelection);
    }

    private void Update () {
        if (control.pause && Statics.GameHasStarted) {
            if (optionsMenuOpen) {
                ToggleOptionsMenu ();
            } else {
                TogglePauseMenu ();
            }
        }
        UpdateCursor ();
    }

    public void TogglePauseMenu () {
        Statics.GameIsPaused = !Statics.GameIsPaused;
        PrimaryMenu.SetActive (Statics.GameIsPaused);
        Time.timeScale = Statics.GameIsPaused ? 0 : 1;
        if (Statics.GameIsPaused) {
            EventSystem.current.SetSelectedGameObject (PrimaryMenuAutoSelection);
        }
    }

    public void ToggleOptionsMenu () {
        optionsMenuOpen = !optionsMenuOpen;
        PrimaryMenu.SetActive (!optionsMenuOpen);
        OptionsMenu.SetActive (optionsMenuOpen);
        EventSystem.current.SetSelectedGameObject (optionsMenuOpen ? OptionsMenuAutoSelection : PrimaryMenuAutoSelection);
        if (!optionsMenuOpen) {
            ApplySettings ();
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

    public void NewGame () {
        SaveLoad.NewGame (0);

        StartGame ();
    }

    public void ContinueGame () {
        if (SaveLoad.savedGames.Length > 0) {
            SaveLoad.Load (0);

        } else {
            Debug.LogWarning ("TODO: try making this button un-interactable rather than making it do nothing");
        }

        StartGame ();
    }

    void StartGame () {
        Statics.GameIsPaused = false;
        Statics.GameHasStarted = true;

        if (GameStart != null) {
            GameStart ();
        }

        PrimaryMenu.SetActive (false);
        OptionsMenu.SetActive (false);
        SaveButton.SetActive (true);
        ContinueButtonImage.sprite = ContinueButtonAltTextSprite;
        ContinueButtonImage.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (ContinueButtonImage.GetComponent<RectTransform> ().anchoredPosition.x, ContinueButtonAltYPosition);
    } 

    public void SaveGame () {
        SaveLoad.Save (0);
    }

    public void UpdateVolume (float value) {
        volume = value;
        AudioManager.AmbienceVolume = volume;
        AudioManager.SFXVolume = volume;
    }

    // Takes a float as input because sliders only support floats as dynamic values
    // The slider that calls this method should use whole numbers to avoid visual inconsistency
    public void UpdateResolution (float index) {
        resolutionIndex = Mathf.RoundToInt (index);
        currentResDisplay.text = ResToString (resolutions [resolutionIndex]);
    }

    string ResToString (Resolution res) {
        return "Resolution: " + res.width + " x " + res.height;
    }

    void ApplySettings () {
        Screen.SetResolution (resolutions [resolutionIndex].width, resolutions [resolutionIndex].height, true);
        AudioManager.AmbienceVolume = volume;
        AudioManager.SFXVolume = volume;
    }

    public void Quit () {
        Application.Quit ();
    }
}
