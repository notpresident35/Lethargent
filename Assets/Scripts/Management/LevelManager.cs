using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelManager //Is started by Main Menu or when game is started
{
    public static LevelManager current; //Starts a global Level Manager

    public PlayerStats playerData; //Holds the current player data

    public CompletionStats completionStats; //Holds the current player's progression

    public float time; // Stores TimeSystem.currentTime 

    public bool isSceneBeingLoaded = false; //Checks if a scene is being loaded

    public LevelManager () {
        playerData = new PlayerStats();
        completionStats = new CompletionStats();
        time = 0;
    }
}
