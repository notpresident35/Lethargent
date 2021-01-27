using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statics {

    //** Layers and Tags

    public static string PlayerTagName = "Player";
    public static int GroundLayer = 8;
    public static int InteractableLayer = 9;
    public static int ObstacleLayer = 10;

    //** Audio

    public static float SFXRange;

    //** Player Prefs

    // Ambience also includes music
    public static string AmbienceVolumePlayerPrefName = "AmbienceVolume";
    public static string SFXVolumePlayerPrefName = "SFXVolume";

    //** Other
    public static float DayLength = 12f; // Day/night cycle length, measured in seconds
    public static float Sqrt2 = 1.4142136f; // Approximation of the square root of 2

    public static float Interpolate (float start, float target, float interpolation) {
        return (start * interpolation) + (target * (1 - interpolation));
    }
}
