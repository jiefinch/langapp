using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
// sort by playerID
public class PlayerData
{
    // for the character
    //public string playerID; firebase already deals w/
    public string username;
    public string lastActive;
    public string nativeLang;
    public string targetLang;
    
    public int coins;
    public int rank;

    public PlayerData(string username, string LA, string NL, string TL, int coins, int rank)
    {
        this.username = username;
        this.lastActive = LA;
        this.nativeLang = NL;
        this.targetLang = TL;
        this.coins = coins;
        this.rank = rank;
    }
}

[Serializable]
public class EquipItems
{
    // implement color later lol

    [Serializable]
    public class Eye
    {
        public Sprite sprite;
        public Color color;
    }

    [Serializable]
    public class Outfit 
    {
        public Sprite armb;
        public Sprite armf;
        public Sprite outfitbase;
        public Color color;
    }

    [Serializable]
    public class Hair 
    {
        public Sprite front;
        public Sprite back;
        public Color color;
    }

    [Serializable]
    public class Accessory 
    {
        public Sprite sprite;
        public Color color;
    }
    
    public List<Outfit> Outfits;
    public List<Hair> Hairs;
    public List<Accessory> Accessories;
    public List<Eye> Eyes;
}

[Serializable]
public class Equipped
{
    public int eyes;
    public int hair;
    public int outfit;
    public List<int> accessories;
}

/*
public class InventoryData
{
    public Dict<> owned;
    public Dict<> equipped;
}*/

// sort by player ID ( using System.Globalization >> DateTime.Today)
public class GameData
{
    // for the daily logs
    // public string playerID; thanks bestie
    // string lastActive;
    public float avgTime;
    public float rank;
    
}

public static class Data
{
    public static readonly Dictionary<string, string> Language
    = new Dictionary<string, string>
    {
        { "English", "en" },
        { "Korean", "ko" },
        { "Indonesian", "id"}
    };
}


public class QuestionData
{

}

