using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerInventory
{
    // State
    public List<CaughtFish> caughtFish = new List<CaughtFish>();

    // Variables
    public int MaxFishStorage => (int)GameManager.GetPlayerStat(StatType.fishStorage);



    // Methods


    public void AddFish(CaughtFish newCatch)
    {
        if (caughtFish.Count < MaxFishStorage)
        {
            caughtFish.Add(newCatch);
          //  Debug.Log($"Added {newCatch.fish.name} to inventory! Current count: {caughtFish.Count}/{MaxFishStorage}");
        }
        else
        {
        //    Debug.Log("Cannot add fish to inventory, storage is full!");
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

    public void RemoveFishQuest(string fishName, int quantity)
    {
        List<int> indexes = new();
        
        //go through our inventory
        for(int i = 0; i < caughtFish.Count;i++)
        {
            if(caughtFish[i].fish.name == fishName)
            {
                indexes.Add(i);
                if(indexes.Count >= quantity)
                {
                    break;
                }
            }
        }
        //the reason I'm doing it like this is so it removes it from the front of the total list, just because I like the look of it more tbh
        for(int i = indexes.Count - 1; i >= 0; i--)
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
 
        foreach(CaughtFish cf in caughtFish)
        {
            if(cf.fish.name == fishName)
            {
                num++;
            }
        }
        return num;
    }
}
