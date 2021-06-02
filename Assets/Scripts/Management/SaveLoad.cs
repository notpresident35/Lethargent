using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static LevelManager[] savedGames = new LevelManager[Statics.SaveFileCount];

    public static event Action SyncDataForSave;
    public static event Action SyncDataOnLoad;

    // Writes all save data to the disk
    public static void WriteToDisk () {
        //Creates save data directory if not created yet.
        if(!Directory.Exists(Application.persistentDataPath+"/Saves")) {
            Directory.CreateDirectory(Application.persistentDataPath+"/Saves");
        }

        BinaryFormatter bf = new BinaryFormatter();

        // 1. Construct a SurrogateSelector object
        SurrogateSelector ss = new SurrogateSelector ();

        ss.AddSurrogate (typeof (Vector3), new StreamingContext (StreamingContextStates.All), new Vector3SerializationSurrogate ());
        ss.AddSurrogate (typeof (Quaternion), new StreamingContext (StreamingContextStates.All), new QuaternionSerializationSurrogate ());

        // 2. Have the formatter use our surrogate selector
        bf.SurrogateSelector = ss;

        FileStream file = File.Create (Application.persistentDataPath + "/Saves/savedGames.gd");
        bf.Serialize(file, savedGames);
        file.Close();
    }

    public static void Save (int index) {
        if (SyncDataForSave != null) {
            SyncDataForSave ();
        }
        savedGames [index] = LevelManager.current;
        WriteToDisk ();
    }

    // Saves a file to savedGames without writing to the disk
    public static void QuickSave (int index) {
        if (SyncDataForSave != null) {
            SyncDataForSave ();
        }
        savedGames [index] = LevelManager.current;
    }

    // Reads all save data from the disk
    public static void LoadFromDisk () {
        if(File.Exists(Application.persistentDataPath + "/Saves/savedGames.gd")) {
            BinaryFormatter bf = new BinaryFormatter();

            // 1. Construct a SurrogateSelector object
            SurrogateSelector ss = new SurrogateSelector ();

            ss.AddSurrogate (typeof (Vector3), new StreamingContext (StreamingContextStates.All), new Vector3SerializationSurrogate ());
            ss.AddSurrogate (typeof (Quaternion), new StreamingContext (StreamingContextStates.All), new QuaternionSerializationSurrogate ());

            // 2. Have the formatter use our surrogate selector
            bf.SurrogateSelector = ss;

            FileStream file = File.Open(Application.persistentDataPath + "/Saves/savedGames.gd", FileMode.Open);
            savedGames = (LevelManager[])bf.Deserialize(file);
            file.Close();
        }
    }

    public static void Load (int index) {
        //SceneManager.LoadScene (0);
        LoadFromDisk ();
        if (savedGames.Length < index) { 
            Debug.LogError ("Attempted to load out-of-bounds save file!");
            NewGameNoSave ();
            return; 
        }
        LevelManager.current = savedGames [index];
        if (SyncDataOnLoad != null) {
            SyncDataOnLoad ();
        }
    }

    // Loads a save file from savedGames without reading from the disk first
    public static void QuickLoad (int index) {
        if (savedGames.Length < index) {
            Debug.LogError ("Attempted to load out-of-bounds save file!");
            NewGameNoSave ();
            return;
        }
        LevelManager.current = savedGames [index];
        if (SyncDataOnLoad != null) {
            SyncDataOnLoad ();
        }
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
