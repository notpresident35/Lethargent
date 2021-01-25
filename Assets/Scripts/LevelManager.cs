using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelManager //Is started by Main Menu or when game is started
{
    public static LevelManager current; //Starts a global Level Manager

    public PlayerStats playerData; //Holds the current player data

    public float time; // Stores TimeSystem.currentTime (TODO: Send this to TimeSystem on game start!)

    public bool isSceneBeingLoaded = false; //Checks if a scene is being loaded

    public LevelManager()
    {
        playerData = new PlayerStats();
        playerData.sceneID = 0;
        playerData.playerPosX = 0;
        playerData.playerPosX = 0;
        playerData.playerPosX = 0;
    }

}
