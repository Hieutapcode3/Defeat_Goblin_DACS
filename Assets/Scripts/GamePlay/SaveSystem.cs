using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static string savePath = Application.persistentDataPath + "/GameData.json";

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public static GameData LoadGame()
    {
        //Debug.Log(savePath);
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            //Debug.Log("Game Loaded!");
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found, creating new data.");
            return new GameData();
        }
    }

}
