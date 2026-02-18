using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory
{
    // State
    public List<CaughtFish> caughtFish = new List<CaughtFish>();

    // Variables
    public int maxFishStorage = 6; // Maximum number of fish the player can store in their inventory

    public int MaxFishStorage => maxFishStorage * statFishStorage;

    // Player stats
    public int statFishStorage;

    public void Init()
    {
        // Grab player stats from GameManager
        statFishStorage = (int)GameManager.GetPlayerStat("fishStorage");
    }

    // Methods
    public void AddFish(CaughtFish newCatch)
    {
        if (caughtFish.Count < MaxFishStorage)
        {
            caughtFish.Add(newCatch);
            Debug.Log($"Added {newCatch.fish.name} to inventory! Current count: {caughtFish.Count}/{MaxFishStorage}");
        }
        else
        {
            Debug.Log("Cannot add fish to inventory, storage is full!");
            // TODO: UI feedback for full inventory
        }
    }

    public void RemoveFish(int index)
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

}
