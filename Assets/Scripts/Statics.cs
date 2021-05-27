using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Statics {

    //** UI/Settings

    public static bool GameIsPaused = false;

    //** Layers and Tags

    public static string PlayerTagName = "Player";
    public static int GroundLayer = 8;
    public static int InteractableLayer = 9;
    public static int ObstacleLayer = 10;

    //** Audio

    public static float SFXRange;
    public static string MasterMixerName = "MasterMixer";
    public static string AmbienceMixerGroupName = "Ambience";
    public static string SFXMixerGroupName = "SFX";

    //** Player Prefs

    // Ambient noise and music are lumped into one "Ambience" category
    public static string AmbienceVolumePlayerPrefName = "AmbienceVolume";
    public static string SFXVolumePlayerPrefName = "SFXVolume";

    //** Other
    public static float Sqrt2 = 1.4142136f; // Approximation of the square root of 2
    public static int Act1CompleteCutsceneID = 4; // TODO: Replace placeholder value once all Act 1 cutscenes are added

    public static float Interpolate (float start, float target, float interpolation) {
        return (start * interpolation) + (target * (1 - interpolation));
    }
}
