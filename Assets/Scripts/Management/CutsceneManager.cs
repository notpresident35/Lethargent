using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour {

    public static CutsceneManager Singleton;

    /* A list of cutscenes and their IDs in order:
     * 
     * Act 1
     * 
     * 0  Opening shot
     * 1  Caught by guard
     * 2  Dragged back to the barracks
     * 3  Returning to the office
     * 4  The big announcement
     * 5  The confrontation
     * 
     * Act 3
     * 
     * 6  Interrogated by spies
     * 7  Confronted by the caretaker
     * 8  Train destroyed by turrets
     * 9  Train destroyed by allied forces
     * 10 Train called off - failure
     * 11 Train called off - success
     * 12 Train reaches the station
     * 13 Execution of a traitor
     * 14 Execution of a rebel
     * 15 𝔈𝔫𝔠𝔬𝔲𝔫𝔱𝔢𝔯 𝔴𝔦𝔱𝔥 𝔞𝔫 𝔲𝔫𝔨𝔫𝔬𝔴𝔞𝔟𝔩𝔢 𝔢𝔫𝔱𝔦𝔱𝔶
     */

    public bool StartGameInCutscene = true;
    public static bool Active;
    public static bool WaitingForContinue;
    public static int CutsceneID;
    public static event Action CutsceneStart;
    public static event Action CutsceneContinue;
    public static event Action CutsceneStop;

    // TODO: Refactor to use the interaction button
    public KeyCode InteractInput;
    public TimelineAsset[] Cutscenes;
    public List<GameObject> Gateways = new List<GameObject> ();
    public List<GameObject> TriggersToEnable = new List<GameObject> ();
    public Image ContinueButton;
    public float ContinueBlinkFrequency;
    public float ContinueBlinkThreshold;

    //Coroutine continueDelayRoutine;
    float lastContinuedTime;
    Color continueButtonColor;

    PlayableDirector director;

    private void Awake () {
        if (Singleton) {
            Destroy (gameObject);
            return;
        } else {
            DontDestroyOnLoad (gameObject);
            Singleton = this;
        }
        director = GetComponent<PlayableDirector> ();
        continueButtonColor = ContinueButton.color;
    }

    private void StartGame () {
        // Sets CutsceneID to first unplayed cutscene
        for (int i = 0; i < LevelManager.current.completionStats.cutscenesWatched.Length; i++) {
            if (LevelManager.current.completionStats.cutscenesWatched [i]) {
                CutsceneID = i;
            } else {
                break;
            }
        }

        if (LevelManager.current.completionStats.cutsceneIsPlaying) {
            StartCutscene (CutsceneID);
        }
    }

    [ContextMenu ("Start Test Cutscene")]
    public void StartTestCutscene () {
        StartCutscene (0);
    }

    private void Update () {
        if (ContinueInput () && Active && WaitingForContinue && !Statics.GameIsPaused) {
            Continue ();
        }

        if (WaitingForContinue) {
            continueButtonColor.a = Mathf.Pow (((Mathf.Sin ((Time.time - lastContinuedTime) * ContinueBlinkFrequency) + 1) * 0.25f) + 0.5f, 2);
        } else {
            continueButtonColor.a = 0;
        }
        ContinueButton.color = continueButtonColor;
    }

    public void Continue () {
        director.Resume ();
        //director.playableGraph.GetRootPlayable (0).SetSpeed (1);
        WaitingForContinue = false;/*
        if (continueDelayRoutine != null) {
            StopCoroutine (continueDelayRoutine);
            continueDelayRoutine = null;
        }*/
        if (CutsceneContinue != null) {
            CutsceneContinue ();
        }
    }

    public void SetContinueFlag () {
        director.Pause ();
        //director.playableGraph.GetRootPlayable (0).SetSpeed (0);
        WaitingForContinue = true;/*
        continueDelayRoutine = StartCoroutine (PromptContinueAfterDelay ());*/
    }

    public void StartCutscene (int ID) {
        Active = true;
        WaitingForContinue = false;/*
        if (continueDelayRoutine != null) {
            StopCoroutine (continueDelayRoutine);
            continueDelayRoutine = null;
        }*/
        CutsceneID = ID;
        CutsceneStart ();
        director.Play (Cutscenes[ID]);
        //print ("starting cutscene");
        LevelManager.current.completionStats.cutsceneIsPlaying = true;
    }

    public void SetCutsceneEnd () {
        Active = false;
        if (Gateways [CutsceneID] != null) {
            Gateways [CutsceneID].SetActive (false);
        }
        if (TriggersToEnable [CutsceneID] != null) {
            TriggersToEnable [CutsceneID].SetActive (true);
        }
        if (LevelManager.current != null) {
            bool [] segments = LevelManager.current.completionStats.completedTutorialSegments;
            segments [CutsceneID] = true;
            LevelManager.current.completionStats.completedTutorialSegments = segments;
        }
        if (CutsceneStop != null) {
            CutsceneStop ();
        }
        LevelManager.current.completionStats.cutsceneIsPlaying = false;
        LevelManager.current.completionStats.cutscenesWatched [CutsceneID] = true;
    }

    public void TriggerContinue () {
        if (CutsceneContinue != null) {
            CutsceneContinue ();
        }
    }

    public bool ContinueInput () {
        return Input.GetKeyDown (InteractInput) || Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1) || Input.GetButtonDown ("Jump") || Input.GetKeyDown (KeyCode.Return);
    }

    private void OnEnable () {
        Menu.GameStart += StartGame;
        SaveLoad.SyncDataOnLoad += SyncDataOnLoad;
    }

    private void OnDisable () {
        Menu.GameStart -= StartGame;
        SaveLoad.SyncDataOnLoad -= SyncDataOnLoad;
    }
    /*
    IEnumerator PromptContinueAfterDelay () {
        yield return new WaitForSeconds (0.05f);
        lastContinuedTime = Time.time;
    }*/

    void SyncDataOnLoad () {
        bool [] progress = LevelManager.current.completionStats.cutscenesWatched;
        int lastWatched = 0;
        for (int i = 0; i < Gateways.Count; i++) {
            if (Gateways [i]) {
                Gateways [i].SetActive (!progress [i]);
            }
            if (progress[i]) {
                lastWatched = i;
            }
        }
        if (TriggersToEnable [lastWatched]) {
            TriggersToEnable [lastWatched].SetActive (true);
        }
    }
} 
