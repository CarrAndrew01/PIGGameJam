using System;
using System.Collections.Generic;
using UnityEngine;

// NOTE: From Nathan -- I was going to make a giant json string to upload to the playerprefs, but I looked into it and this 'JsonUtility' is a little easier. If you need more things saved, just add them here.
[Serializable]
public class SavedFish
{
    public string fishTypeName;
    public float weight;
    public string planetOfOrigin;
}

[Serializable]
public class SavedBait
{
    public string upgradeName;
    public int uses;
    public string mechanicalDescription;
}

[Serializable]
public class InventorySave
{
    public int saveVersion = 1;
    public List<SavedFish> caughtFish = new List<SavedFish>();
    public List<SavedBait> baits = new List<SavedBait>();
    public string currentBaitUpgradeName;
    public float money = 0;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static InventorySave FromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        return JsonUtility.FromJson<InventorySave>(json);
    }
}

[Serializable]
public class PlayerStatsSave
{
    public int saveVersion = 1;
    // Only store the applied upgrade identifiers (names). The concrete stats can be re-derived from Upgrade assets on load.
    public List<string> appliedUpgradeNames = new List<string>();

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static PlayerStatsSave FromJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        return JsonUtility.FromJson<PlayerStatsSave>(json);
    }
}
