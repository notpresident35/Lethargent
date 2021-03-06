using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static List<LevelManager> savedGames = new List<LevelManager>();

    // Writes all save data to the disk
    public static void WriteToDisk () {
        //Creates save data directory if not created yet.
        if(!Directory.Exists(Application.persistentDataPath+"/Saves")) {
            Directory.CreateDirectory(Application.persistentDataPath+"/Saves");
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create (Application.persistentDataPath + "/Saves/savedGames.gd");
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
    }

    public static void Save (int index) {
        savedGames [index] = LevelManager.current;
        WriteToDisk ();
    }

    // Saves a file to savedGames without writing to the disk
    public static void QuickSave (int index) {
        savedGames [index] = LevelManager.current;
    }

    // Reads all save data from the disk
    public static void LoadFromDisk () {
        if(File.Exists(Application.persistentDataPath + "/Saves/savedGames.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Saves/savedGames.gd", FileMode.Open);
            SaveLoad.savedGames = (List<LevelManager>)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void Load (int index) {
        LoadFromDisk ();
        if (savedGames.Count < index) { 
            Debug.LogError ("Attempted to load out-of-bounds save file!");
            NewGameNoSave ();
            return; 
        }
        LevelManager.current = savedGames [index];
    }

    // Loads a save file from savedGames without reading from the disk first
    public static void QuickLoad (int index) {
        if (savedGames.Count < index) {
            Debug.LogError ("Attempted to load out-of-bounds save file!");
            NewGameNoSave ();
            return;
        }
        LevelManager.current = savedGames [index];
    }

    // Used as a fallback; try not to use this method
    public static void NewGameNoSave () {
        LevelManager.current = new LevelManager ();
    }

    public static void NewGame (int saveIndex) {
        LevelManager.current = new LevelManager ();
        Save (saveIndex);
    }
}
