using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneManager : MonoBehaviour {

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
    public KeyCode ContinueInput;
    public TimelineAsset[] Cutscenes;

    PlayableDirector director;

    private void Awake () {
        director = GetComponent<PlayableDirector> ();
    }

    private void Start () {
        // TODO: Check if the game was saved in a cutscene, then play it if it was.
        if (StartGameInCutscene) {
            StartCutscene (0);
        }
    }

    [ContextMenu ("Start Test Cutscene")]
    public void StartTestCutscene () {
        StartCutscene (0);
    }

    private void Update () {
        if ((Input.GetKeyDown (ContinueInput) || Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown (1)) && Active && WaitingForContinue) {
            Continue ();
        }
    }

    public void Continue () {
        director.Resume ();
        //director.playableGraph.GetRootPlayable (0).SetSpeed (1);
        WaitingForContinue = false;
        if (CutsceneContinue != null) {
            CutsceneContinue ();
        }
    }

    public void SetContinueFlag () {
        director.Pause ();
        //director.playableGraph.GetRootPlayable (0).SetSpeed (0);
        WaitingForContinue = true;
    }

    public void StartCutscene (int ID) {
        Active = true;
        WaitingForContinue = false;
        TimeSystem.StopTime ();
        CutsceneStart ();
        director.Play (Cutscenes[ID]);
    }

    public void SetCutsceneEnd () {
        Active = false;
        CutsceneStop ();
        TimeSystem.StartTime ();
    }

    public void TriggerContinue () {
        if (CutsceneContinue != null) {
            CutsceneContinue ();
        }
    }
}
