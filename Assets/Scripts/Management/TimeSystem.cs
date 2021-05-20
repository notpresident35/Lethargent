using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour {

    public static float CurrentTime { get; private set; }
    public static bool IsTimeProgressing { get; private set; } = false;
    public static bool IsAct1Complete { get; private set; } = false; // TODO: Move this

    public static TimeSystem Singleton;

    public AnimationCurve FogStrength; // One horizontal unit equals one day, and vertical units are scaled down 100x
    public float DayLength = 120f; // Day/night cycle length, measured in seconds

    [SerializeField] float startTime = 0.4f;
    [SerializeField] Transform clockNeedle;

    private bool timeProgressingCache;
    private float clockNeedleStartRotation;

    // Needs to run before other scripts on game start, because other scripts depend on the time
    void StartGame () {
        // TODO: Load time from save
        CurrentTime = startTime * DayLength;
        IsTimeProgressing = IsAct1Complete;
        clockNeedleStartRotation = clockNeedle.transform.localRotation.eulerAngles.z;
    }

    /*
    [ContextMenu ("Print current time")]
    void printTime () {
        print (GetCurrentTime ());
    }

    [ContextMenu ("Toggle time")]
    void toggleTime () {
        isTimeProgressing = !isTimeProgressing;
    }*/

    private void Awake () {
        Singleton = GameController.Singleton.GetComponent<TimeSystem> ();
        CutsceneManager.CutsceneStart += StartCutscene;
        CutsceneManager.CutsceneStop += StopCutscene;
        StartGame ();
        // TODO: Make time system start at the end of Act 1
        //StartTime ();
    }


    private void Update () {
        if (IsTimeProgressing) {
            CurrentTime += Time.deltaTime;
            if (clockNeedle) {
                clockNeedle.transform.localRotation = Quaternion.Euler (0, 0, clockNeedleStartRotation - CurrentTime / DayLength * 360f + 180);
            }
        }
        RenderSettings.fogDensity = FogStrength.Evaluate (CurrentTime / DayLength) / 100f;

        // "Reveals" CurrentTime in inspector by setting the object's position
        transform.position = Vector3.one * CurrentTime;
    }

    public static void StopTime () {
        IsTimeProgressing = false;
    }

    [ContextMenu ("Start time")]
    public void DebugStartTime () {
        IsTimeProgressing = true;
    }

    public static void StartTime () {
        IsTimeProgressing = true;
    }

    public static void PauseTime (float time) {
        IsTimeProgressing = false;
        Singleton.StartCoroutine (Singleton.ResumeTime (time));
    }

    [ContextMenu ("Skip 1 day")]
    public void Skiptime () {
        CurrentTime += DayLength;
    }

    IEnumerator ResumeTime (float time) {
        yield return new WaitForSeconds (time);
        IsTimeProgressing = true;
    }

    void StartCutscene () {
        timeProgressingCache = IsTimeProgressing;
        IsTimeProgressing = false;
    }

    void StopCutscene () {
        if (CutsceneManager.CutsceneID == Statics.Act1CompleteCutsceneID) {
            timeProgressingCache = true;
        }
        IsTimeProgressing = timeProgressingCache;
    }
}

