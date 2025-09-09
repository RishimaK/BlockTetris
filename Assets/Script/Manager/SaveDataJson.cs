using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SaveDataJson : MonoBehaviour
{
    private PlayerData playerData;
    private string filePath;
    private PlayerData data;

    private Dictionary<string, Func<object>> playerDataMap;


    [Header("Map Data")]
    public TextAsset jsonMapData;
    public MapList MapData = new MapList();

    [Serializable]public class Map
    {
        public int MapName;
        public int[][] BlockList;
        public string[] ListFind;
        public int[] ValueFind;
        public int[] RandomRange;
    }
    [Serializable]public class MapList
    {
        public Map[] map;
    }


    [Header("Map Shape List")]
    public TextAsset MapShapeListJson;
    public MapShapeListData MapShapeList = new MapShapeListData();

    [Serializable]public class MapShape
    {
        public int[][] Map;
    }
    [Serializable]public class MapShapeListData
    {
            public MapShape[] MapList;
    }
    
    void Awake()
    {
        MapData = JsonConvert.DeserializeObject<MapList>(jsonMapData.text);
        MapShapeList = JsonConvert.DeserializeObject<MapShapeListData>(MapShapeListJson.text);

        playerData = PlayerData.Instance;
        string nameGame = "TripleTetris";
        filePath = Application.dataPath + Path.AltDirectorySeparatorChar + $"{nameGame}_PlayerData.json";
        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = Path.Combine(Application.persistentDataPath, $"{nameGame}_PlayerData.json");
        }

        if(!File.Exists(filePath)) SaveNewData();
        else LoadData();

        InitializePlayerDataMap();
    }
    
    private void SaveNewData()
    {
        SaveDataToJsonFile();
        LoadData();
    }

    private void SaveDataToJsonFile()
    {
        data = playerData;
        string json = JsonUtility.ToJson(playerData);

        using(StreamWriter writer = new StreamWriter(filePath)) writer.Write(json);
    }

    private void LoadData()
    {
        string json = File.ReadAllText(filePath);
        // string json = string.Empty;
        // using(StreamReader reader = new StreamReader(filePath)) json = reader.ReadToEnd();
        if(json == ""){
            //Nếu file json rỗng
            SaveNewData();
            return;
        }   

        data = JsonUtility.FromJson<PlayerData>(json);

        playerData.SetPlayerData
        (
            data.RemoveAds, data.Rate, data.Music, data.Sound, data.Vibration
        );

        // return data;
    }

    private void InitializePlayerDataMap()
    {
        playerDataMap = new Dictionary<string, Func<object>>
        {
            { "RemoveAds", () => data.RemoveAds},
            { "Rate", () => data.Rate},
            { "Music", () => data.Music},
            { "Sound", () => data.Sound},
            { "Vibration", () => data.Vibration},
            // { "TakeToolTutorial", () => data.TakeToolTutorial},
            // { "Butterfly", () => data.Butterfly},
            // { "Star", () => data.Star},
            // { "Gold", () => data.Gold},
            // { "Boom", () => data.Boom},
            // { "TNT", () => data.TNT},
            // { "Hammer", () => data.Hammer},
            // { "Rainbow", () => data.Rainbow},
            // { "OpenedMap", () => data.OpenedMap},
            // { "Challenge", () => data.Challenge},
            // { "CurrentTheme", () => data.CurrentTheme},
            // { "DailyReward", () => data.DailyReward},
            // { "DailyRewardStack", () => data.DailyRewardStack},
            // { "PiggyBank", () => data.PiggyBank},
            // { "ChestStar", () => data.ChestStar},
        };
    }

    
    public object TakePlayerData(string name)
    {
        if (playerDataMap.TryGetValue(name, out var getter))
        {
            return getter();
        }
        return null;
    }


    public MapList TakeMapData()
    {
        return MapData;
    }
    
    public MapShapeListData TakeMapShapeList()
    {
        return MapShapeList;
    }

    public void SaveData(string name, object val, int mapNum = 0)
    {
        // lưu dữ liệu tại thời gian thực
        if (name == "AddMoreItem" || name == "ShowAllItem") playerData.SetPlayerData(name, val, mapNum);
        else playerData.SetPlayerData(name, val);
        SaveDataToJsonFile();
    }

    public void RemoveItemFromList (string name, string val)
    {
        playerData.RemoveItemFromList(name, val);
        SaveDataToJsonFile();
    }

    public void ChangeChallengeRemainingTime (int index, int val)
    {
        playerData.ChangeChallengeRemainingTime(index, val);
        SaveDataToJsonFile();
    }

    public object GetData(string name)
    {
        // lấy 1 dữ liệu cự thể đã lưu
        object  player = TakePlayerData(name);
        return player;
    }

    public PlayerData GetData()
    {
        // lấy toàn bộ dữ liệu đã lưu
        return data;
    }
}
