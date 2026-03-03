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
    public int cost;

    public Bait(Upgrade upgrade, int uses, int cost = 0)
    {
        baitUpgrade = upgrade;
        numberOfUses = uses;
        this.cost = cost;
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

    public int maxInt = 0;
    public float money = 0; // Player's current money, 

    public int NewID()
    {
        maxInt++;
        return maxInt;
    }


    // Methods
    public void ClearInventory()
    {
        caughtFish.Clear();
        baits.Clear();
        currentBaitUpgrade = null;
        money = 0;
        maxInt = 0;
    }
    
    public void AddBait(Upgrade baitUpgrade, int uses)
    {
        int existingIndex = baits.FindIndex(b => b.baitUpgrade == baitUpgrade);

        if (existingIndex != -1)
        {
            // If the bait already exists, update its uses
            baits[existingIndex] = new Bait(baitUpgrade, baits[existingIndex].numberOfUses + uses);
        }
        else
        {
            baits.Add(new Bait(baitUpgrade, uses));
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
                baits[existingIndex] = new Bait(baitUpgrade, newUses);
            }
            else
            {
                // Otherwise, remove the bait from the inventory
                baits.RemoveAt(existingIndex);
                // If the removed bait was currently selected, deselect it
                if (currentBaitUpgrade == baitUpgrade)
                {
                    DeselectBait();
                }
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
            if (currentBaitUpgrade != null)
            {
                // If we already have a bait selected, remove its upgrade effects before switching
                GameManager.RemoveUpgrade(currentBaitUpgrade);
            }

            currentBaitUpgrade = baitUpgrade;
            // Apply the bait as an active upgrade
            GameManager.AddUpgrade(baitUpgrade);
        }
        else
        {
            Debug.LogWarning($"Attempted to select bait {baitUpgrade.name} which is not in inventory.");
        }
    }

    public void DeselectBait()
    {
        if (currentBaitUpgrade != null)
        {
            GameManager.RemoveUpgrade(currentBaitUpgrade);
            currentBaitUpgrade = null;
        }
    }


    public void AddFish(CaughtFish newCatch)
    {
        if (caughtFish.Count < MaxFishStorage)
        {
            // Assign a unique ID when the fish is actually added to inventory
            newCatch.id = NewID();
            caughtFish.Add(newCatch);
        }
        else
        {
            Toast.ShowToast("Inventory Full! Cannot add fish.");
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
            Debug.Log($"Removed {catchToRemove.fish.fishName} from inventory. Current count: {caughtFish.Count}/{MaxFishStorage}");
        }
        else
        {
            Debug.LogError($"Attempted to remove {catchToRemove.fish.fishName} which is not in inventory!");
        }
    }

    public void RemoveFishByID(int id)
    {

        for (int i = 0; i < caughtFish.Count; i++)
        {
            if (caughtFish[i].id == id)
            {
                RemoveFishAt(i);
                return; // stop after removing the matched fish
            }
        }
        Debug.LogWarning($"Attempted to remove fish with id {id}, but none was found.");
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
                uses = b.numberOfUses
            });
        }

        save.currentBaitUpgradeName = currentBaitUpgrade != null ? currentBaitUpgrade.name : "";
        save.money = money;

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
            // Use AddFish so IDs are assigned consistently and storage limits are respected
            AddFish(cf);
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
            baits.Add(new Bait(up, sb.uses));
        }

        currentBaitUpgrade = !string.IsNullOrEmpty(save.currentBaitUpgradeName)
            ? GameManager.FindUpgradeByName(save.currentBaitUpgradeName)
            : null;

        // Restore saved money
        money = save.money;

        // Ensure maxInt reflects the highest assigned ID in the inventory so
        // subsequent calls to NewID() generate unique IDs above existing ones.
        int highestId = 0;
        for (int i = 0; i < caughtFish.Count; i++)
        {
            if (caughtFish[i].id > highestId) highestId = caughtFish[i].id;
        }
        // If IDs were never assigned (all zeros), fall back to using the current count
        if (highestId == 0)
            maxInt = caughtFish.Count;
        else
            maxInt = highestId;
    }
}
