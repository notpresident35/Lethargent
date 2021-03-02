using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour {

    public static float CurrentTime { get; private set; }
    public static bool IsTimeProgressing { get; private set; } = false;
    public static bool IsAct1Complete { get; private set; } = false; // TODO: Move this

    static TimeSystem Singleton;

    public AnimationCurve FogStrength; // One horizontal unit equals one day, and vertical units are scaled down 100x

    // Needs to run before other scripts on game start, because other scripts depend on the time
    [ContextMenu ("Start game")]
    void StartGame () {
        // TODO: Load time from save
        CurrentTime = 0;
        IsTimeProgressing = IsAct1Complete;
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
        if (!Singleton) {
            Singleton = this;
        } else {
            Debug.LogWarning ("Only one instance of the TimeSystem should be used; remove all duplicates!");
            Destroy (this);
        }
        StartGame ();
    }


    private void Update () {
        if (IsTimeProgressing) {
            CurrentTime += Time.deltaTime;
        }
        RenderSettings.fogDensity = FogStrength.Evaluate (CurrentTime / Statics.DayLength) / 100f;
    }

    public static void StopTime () {
        IsTimeProgressing = false;
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
        CurrentTime += Statics.DayLength;
    }

    IEnumerator ResumeTime (float time) {
        yield return new WaitForSeconds (time);
        IsTimeProgressing = true;
    }
}

