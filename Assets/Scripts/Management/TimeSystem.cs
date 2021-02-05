using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour {

    public static float currentTime { get; private set; }
    public static bool isTimeProgressing { get; private set; } = false;

    static TimeSystem Singleton;

    public AnimationCurve FogStrength; // One horizontal unit equals one day, and vertical units are scaled down 100x

    // Needs to run before other scripts on game start, because other scripts depend on the time
    [ContextMenu ("Start game")]
    void StartGame () {
        // TODO: Load time from save
        currentTime = 0;
        isTimeProgressing = true;
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
        if (isTimeProgressing) {
            currentTime += Time.deltaTime;
        }
        RenderSettings.fogDensity = FogStrength.Evaluate (currentTime / Statics.DayLength) / 100f;
    }

    public static void StopTime () {
        isTimeProgressing = false;
    }

    public static void StartTime () {
        isTimeProgressing = true;
    }

    public static void PauseTime (float time) {
        isTimeProgressing = false;
        Singleton.StartCoroutine (Singleton.ResumeTime (time));
    }

    [ContextMenu ("Skip 1 day")]
    public void Skiptime () {
        currentTime += Statics.DayLength;
    }

    IEnumerator ResumeTime (float time) {
        yield return new WaitForSeconds (time);
        isTimeProgressing = true;
    }
}

