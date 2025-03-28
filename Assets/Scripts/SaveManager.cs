using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveManager
{
    public static string savePath = Application.persistentDataPath + "/save.dat";

    public static void SaveGame(int score, int bestScore, List<Cell> cells)
    {
        BinaryFormatter formatter = new();
        FileStream file = File.Create(savePath);

        GameData data = new(score, bestScore, cells);
        formatter.Serialize(file, data);
        file.Close();

        Debug.Log("Game saved");
    }

    public static GameData LoadGame()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new();
            FileStream file = File.Open(savePath, FileMode.Open);

            GameData data = (GameData)formatter.Deserialize(file);
            file.Close();

            Debug.Log("Game loaded");
            return data;
        }

        Debug.LogWarning("No save file found");
        return null;
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save deleted");
        }
    }
}
