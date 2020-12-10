using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static string SavePath = Application.persistentDataPath + "/GameData.json";

    public static void SaveData(GameData gameData)
    {
        string sGameData = JsonUtility.ToJson(gameData);
        System.IO.File.WriteAllText(SavePath , sGameData);
    }
    
    public static GameData LoadData()
    {
        string json = System.IO.File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameData>(json);
    }
}

[System.Serializable]
public class GameData
{
    public int coins;
    public int highscore;
    public List<string> baskets = new List<string>();
    public string selected;
}
