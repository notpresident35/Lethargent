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
}
