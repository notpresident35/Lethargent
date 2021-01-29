using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutsceneManager : MonoBehaviour {

    public static bool Active;
    public static bool CanContinue;
    public static int CutsceneID;

    // TODO: Refactor kinda hacky debug code to use the interaction button
    public KeyCode ContinueInput;

    PlayableDirector director;

    private void Awake () {
        director = GetComponent<PlayableDirector> ();
    }

    [ContextMenu ("Start Test Cutscene")]
    public void StartTestCutscene () {
        Active = true;
        CanContinue = false;
        TimeSystem.StopTime ();
        director.Play ();
    }

    private void Update () {
        if (Input.GetKeyDown (ContinueInput) && Active && CanContinue) {
            Continue ();
        }
    }

    public void Continue () {
        director.playableGraph.GetRootPlayable (0).SetSpeed (1);
        CanContinue = false;
    }

    public void SetContinueFlag () {
        director.playableGraph.GetRootPlayable (0).SetSpeed (0);
        TimeSystem.StartTime ();
        CanContinue = true;
    }

    public void SetCutsceneEnd () {
        Active = false;
    }
}
