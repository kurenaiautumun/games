using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Windows;
using System.IO;
using Newtonsoft.Json;


public static class SaveSystem 
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves";
    public const string FILE_NAME = "SaveFile";
    private const string SAVE_EXTENSION = ".sav";
    public static string fileName { get; private set;}
    public static string filePath { get; private set;}

    public static void Initailize()
    {
        if(!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        fileName = FILE_NAME + SAVE_EXTENSION;
        filePath = SAVE_FOLDER + FILE_NAME + SAVE_EXTENSION;

    }

    public static void Save(SaveData saveObject)
    {
        var settings = new JsonSerializerSettings();
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        string saveString = JsonConvert.SerializeObject(saveObject, settings);
        Debug.Log("Saved string: " + saveString);
        File.WriteAllText(filePath, saveString);
    }

    public static SaveData Load()
    {
        if(File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            Debug.Log("Loaded String: " + saveString);
            SaveData loaded = JsonConvert.DeserializeObject<SaveData>(saveString);
            if(loaded == null)
            {
                return new SaveData();
            }

            return loaded;

        }
        else
        {
            return new SaveData();
        }
    }
}
