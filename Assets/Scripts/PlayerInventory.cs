using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Represents a number of bait items in the inventory.
/// </summary>
[Serializable]
public struct Bait
{
    public Upgrade baitUpgrade;
    public int numberOfUses;
    public string mechanicalDescription;

    public Bait(Upgrade upgrade, int uses, string description)
    {
        baitUpgrade = upgrade;
        numberOfUses = uses;
        mechanicalDescription = description;
    }
}

/// <summary>
/// Manages the player's inventory. Mostly just fish and bait.
/// </summary>
[Serializable]
public class PlayerInventory
{
    // State
    public List<CaughtFish> caughtFish = new List<CaughtFish>();
    public List<Bait> baits = new List<Bait>();
    public Upgrade currentBaitUpgrade; // Reference to the currently equipped bait upgrade, if any

    // Variables
    public int MaxFishStorage => (int)GameManager.GetPlayerStat(StatType.fishStorage);

    // Methods
    public void AddBait(Upgrade baitUpgrade, int uses, string description)
    {
        int existingIndex = baits.FindIndex(b => b.baitUpgrade == baitUpgrade);

        if (existingIndex != -1)
        {
            // If the bait already exists, update its uses
            baits[existingIndex] = new Bait(baitUpgrade, baits[existingIndex].numberOfUses + uses, description);
        }
        else
        {
            baits.Add(new Bait(baitUpgrade, uses, description));
        }
    }

    public void RemoveBait(Upgrade baitUpgrade, int uses)
    {
        int existingIndex = baits.FindIndex(b => b.baitUpgrade == baitUpgrade);

        if (existingIndex != -1)
        {
            Bait existingBait = baits[existingIndex];
            int newUses = existingBait.numberOfUses - uses;

            if (newUses > 0)
            {
                // As long as we still have some uses left, update the bait
                baits[existingIndex] = new Bait(baitUpgrade, newUses, existingBait.mechanicalDescription);
            }
            else
            {
                // Otherwise, remove the bait from the inventory
                baits.RemoveAt(existingIndex);
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to remove bait {baitUpgrade.name} which is not in inventory.");
        }
    }

    public void SelectBait(Upgrade baitUpgrade)
    {
        if (baits.Exists(b => b.baitUpgrade == baitUpgrade))
        {
            currentBaitUpgrade = baitUpgrade;
        }
        else
        {
            Debug.LogWarning($"Attempted to select bait {baitUpgrade.name} which is not in inventory.");
        }
    }


    public void AddFish(CaughtFish newCatch)
    {
        if (caughtFish.Count < MaxFishStorage)
        {
            caughtFish.Add(newCatch);
        }
        else
        {
            // TODO: UI feedback for full inventory
        }
    }

    public void RemoveFishAt(int index)
    {
        if (index >= 0 && index < caughtFish.Count)
        {
            caughtFish.RemoveAt(index);
            Debug.Log($"Removed fish at index {index} from inventory. Current count: {caughtFish.Count}/{MaxFishStorage}");
        }
        else
        {
            Debug.LogError($"Invalid index {index} for removing fish from inventory!");
        }
    }
    public void RemoveFish(CaughtFish catchToRemove)
    {
        if (caughtFish.Remove(catchToRemove))
        {
            Debug.Log($"Removed {catchToRemove.fish.name} from inventory. Current count: {caughtFish.Count}/{MaxFishStorage}");
        }
        else
        {
            Debug.LogError($"Attempted to remove {catchToRemove.fish.name} which is not in inventory!");
        }
    }

    public void SellFish(int index)
    {
        RemoveFishAt(index);
        GameManager.Instance.money += caughtFish[index].weight * 10; // Example: sell price based on weight

    }


    public void RemoveFishQuest(string fishName, int quantity)
    {
        List<int> indexes = new();

        //go through our inventory
        for (int i = 0; i < caughtFish.Count; i++)
        {
            if (caughtFish[i].fish.name == fishName)
            {
                indexes.Add(i);
                if (indexes.Count >= quantity)
                {
                    break;
                }
            }
        }
        //the reason I'm doing it like this is so it removes it from the front of the total list, just because I like the look of it more tbh
        for (int i = indexes.Count - 1; i >= 0; i--)
        {
            caughtFish.RemoveAt(indexes[i]);
        }
    }

    public Dictionary<string, int> GetFishCountsByType()
    {
        Dictionary<string, int> fishCounts = new Dictionary<string, int>();
        foreach (CaughtFish fish in caughtFish)
        {
            string fishName = fish.fish.name;
            if (fishCounts.ContainsKey(fishName))
                fishCounts[fishName]++;
            else
                fishCounts[fishName] = 1;
        }
        return fishCounts;
    }

    public int NumberOfFish(string fishName)
    {
        int num = 0;

        foreach (CaughtFish cf in caughtFish)
        {
            if (cf.fish.name == fishName)
            {
                num++;
            }
        }
        return num;
    }

    // Persistence
    public void SaveToPlayerPrefs()
    {
        InventorySave save = new InventorySave();

        foreach (var cf in caughtFish)
        {
            save.caughtFish.Add(new SavedFish
            {
                fishTypeName = cf.fish != null ? cf.fish.name : "",
                weight = cf.weight,
                planetOfOrigin = cf.planetOfOrigin
            });
        }

        foreach (var b in baits)
        {
            save.baits.Add(new SavedBait
            {
                upgradeName = b.baitUpgrade != null ? b.baitUpgrade.name : "",
                uses = b.numberOfUses,
                mechanicalDescription = b.mechanicalDescription
            });
        }

        save.currentBaitUpgradeName = currentBaitUpgrade != null ? currentBaitUpgrade.name : "";

        string json = save.ToJson();
        PlayerPrefs.SetString("Manager_Inventory", json);
        PlayerPrefs.Save();
    }

    public void LoadFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("Manager_Inventory")) return;

        string json = PlayerPrefs.GetString("Manager_Inventory");
        InventorySave save = InventorySave.FromJson(json);
        if (save == null) return;

        caughtFish.Clear();
        baits.Clear();

        foreach (var sf in save.caughtFish)
        {
            if (string.IsNullOrEmpty(sf.fishTypeName)) continue;
            Fish fishType = GameManager.FindFishTypeByName(sf.fishTypeName);
            if (fishType == null)
            {
                Debug.LogWarning($"Saved fish type not found: {sf.fishTypeName}");
                continue;
            }
            CaughtFish cf = new CaughtFish(fishType, sf.weight, sf.planetOfOrigin);
            caughtFish.Add(cf);
        }

        foreach (var sb in save.baits)
        {
            if (string.IsNullOrEmpty(sb.upgradeName)) continue;
            Upgrade up = GameManager.FindUpgradeByName(sb.upgradeName);
            if (up == null)
            {
                Debug.LogWarning($"Saved bait upgrade not found: {sb.upgradeName}");
                continue;
            }
            baits.Add(new Bait(up, sb.uses, sb.mechanicalDescription));
        }

        currentBaitUpgrade = !string.IsNullOrEmpty(save.currentBaitUpgradeName)
            ? GameManager.FindUpgradeByName(save.currentBaitUpgradeName)
            : null;
    }
}
