using System;
using System.Collections.Generic;

public class PlayerData
{
    public bool RemoveAds = false;
    public bool Rate = false;
    public bool Music = true;
    public bool Sound = true;
    public bool Vibration = true;
    // public bool TakeToolTutorial = false;

    // public int Butterfly = 0;
    // public int Star = 0;
    // public int Gold = 0;
    // public int Boom = 0;
    // public int TNT = 0;
    // public int Hammer = 0;
    // public int Rainbow = 0;
    // public int OpenedMap = 1;
    // public int Challenge = 1;
    // public int CurrentTheme = 1;

    // public string DailyReward = null;
    // public int DailyRewardStack = 0;

    // public int PiggyBank = 0;
    // public int ChestStar = 0;

    private static readonly Lazy<PlayerData> _instance = new Lazy<PlayerData>(() => new PlayerData());
    public static PlayerData Instance => _instance.Value;

    private PlayerData() { }

    public void SetPlayerData
    (
        bool removeAds = false, bool rate = false, bool music = true, bool sound = true, bool vibration = true, bool takeToolTutorial = false,
        int butterfly = 0, int star = 0,int gold = 0, int boom = 0, int tnt = 0, int hammer = 0, int rainbow = 0, int openedMap = 1, int challenge = 1, int currentTheme = 1,
        string dailyReward = null, int dailyRewardStack = 0, int piggyBank = 0, int chestStar = 0
    )
    {
        RemoveAds = removeAds;
        Rate = rate;
        Music = music;
        Sound = sound;
        Vibration = vibration;
        // TakeToolTutorial = takeToolTutorial;

        // Butterfly = butterfly;
        // Star = star;
        // Gold = gold;
        // Boom = boom;
        // TNT = tnt;
        // Hammer = hammer;
        // Rainbow = rainbow;

        // OpenedMap = openedMap;
        // Challenge = challenge;
        // CurrentTheme = currentTheme;

        // DailyReward = dailyReward;
        // DailyRewardStack = dailyRewardStack;

        // PiggyBank = piggyBank;
        // ChestStar = chestStar;

        // ChallengeRemainingTime = challengeRemainingTime?.ToList() ?? new List<int>();
        // CompletedChallengeMap = completedChallengeMap?.ToList() ?? new List<int>();
        // ListItemSlotChallenge1 = listItemSlotChallenge1?.ToList() ?? new List<string>();
        // ListChallenge1 = listChallenge1?.ToList() ?? new List<string>();

        // OpenedMiniGameMap = openedMiniGameMap;
        // AddMoreItem = addMoreItem;
        // ShowAllItem = showAllItem;
        // ItemMap1 = itemMap1?.ToList() ?? new List<string>();
        // ShowHiddenItems1 = showHiddenItems1?.ToList() ?? new List<string>();
    }

    public void SetPlayerData<T>(string name, T val)
    {
        try
        {
            switch (name)
            {
                case nameof(RemoveAds): RemoveAds = (bool)(object)val; break;
                case nameof(Rate): Rate = (bool)(object)val; break;
                case nameof(Music): Music = (bool)(object)val; break;
                case nameof(Sound): Sound = (bool)(object)val; break;
                case nameof(Vibration): Vibration = (bool)(object)val; break;
                // case nameof(TakeToolTutorial): TakeToolTutorial = (bool)(object)val; break;
                // case nameof(Butterfly): Butterfly = (int)(object)val; break;
                // case nameof(Star): Star = (int)(object)val; break;
                // case nameof(Gold): Gold = (int)(object)val; break;
                // case nameof(Boom): Boom = (int)(object)val; break;
                // case nameof(TNT): TNT = (int)(object)val; break;
                // case nameof(Hammer): Hammer = (int)(object)val; break;
                // case nameof(Rainbow): Rainbow = (int)(object)val; break;
                // case nameof(OpenedMap): OpenedMap = (int)(object)val; break;
                // case nameof(Challenge): Challenge = (int)(object)val; break;
                // case nameof(CurrentTheme): CurrentTheme = (int)(object)val; break;
                // case nameof(DailyReward): DailyReward = val as string; break;
                // case nameof(DailyRewardStack): DailyRewardStack = (int)(object)val; break;
                // case nameof(PiggyBank): PiggyBank = (int)(object)val; break;
                // case nameof(ChestStar): ChestStar = (int)(object)val; break;


                // case nameof(ChallengeRemainingTime): AddItemToList(ChallengeRemainingTime, (int)(object)val); break;
                // case nameof(CompletedChallengeMap): AddItemToList(CompletedChallengeMap, (int)(object)val); break;
                
                // case nameof(ListItemSlotChallenge1): AddItemToList(ListItemSlotChallenge1, (string)(object)val); break;
                // case nameof(ListChallenge1): AddItemToList(ListChallenge1, (string)(object)val); break;
                // case nameof(ItemMap1): AddItemToList(ItemMap1, val as string); break;
                // case nameof(ShowHiddenItems1): AddItemToList(ShowHiddenItems1, val as string); break;

                default: throw new ArgumentException($"Invalid property name: {name}");
            }
        }
        catch (InvalidCastException ex)
        {
            throw new ArgumentException($"Invalid value type for property {name}", ex);
        }
    }

    public void SetPlayerData(string name, object val, int mapNum)
    {
        switch (name)
        {
            // case "AddMoreItem": AddItemToList(AddMoreItem, val, name, mapNum); break;
            // case "ShowAllItem": AddItemToList(ShowAllItem, val, name, mapNum); break;
            default: throw new ArgumentException($"Invalid property name: {name}");
        }
    }

    public void AddItemToList(bool[]itemMap, object val, string name, int mapNum)
    {
        if(itemMap == null) itemMap = new bool[] {};
        if(val != null){
            int length = itemMap.Length;
            if(length < mapNum){
                Array.Resize(ref itemMap, mapNum);
                itemMap[mapNum - 1] = (bool)val;
            } else itemMap[mapNum - 1] = (bool)val;
        } else itemMap = new bool[] {};

        // if(name == "AddMoreItem") this.AddMoreItem = itemMap;
        // else if(name == "ShowAllItem") this.ShowAllItem = itemMap;
    }

    private void AddItemToList<T>(List<T> list, T item)
    {
        if(item == null) list.Clear();
        else if (item != null && !list.Contains(item))
        {
            list.Add(item);
        }
    }

    public void RemoveItemFromList (string list, string item)
    {
        // if(list == "ListItemSlotChallenge1") ListItemSlotChallenge1.Remove(item);
        // else if(list == "ListItemSlotChallenge2") ListItemSlotChallenge2.Remove(item);
        // else if(list == "ListItemSlotChallenge3") ListItemSlotChallenge3.Remove(item);
        // else if(list == "ListItemSlotChallenge4") ListItemSlotChallenge4.Remove(item);
        // else if(list == "ListItemSlotChallenge5") ListItemSlotChallenge5.Remove(item);
    }

    public void ChangeChallengeRemainingTime (int index, int item)
    {
        // ChallengeRemainingTime[index] = item;
    }
}