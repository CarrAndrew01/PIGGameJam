using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public string questName = "";
    public string description = "";

    public enum QuestType
    {
        QuantityFish, // Catch a certain type of fish
        SpecificFish, //catch a rare variant fish
    }

    public QuestType type;

    /*****
    FOR QUANTITY OF FISH QUESTS:
    *******/
    public int quantity = 0; // For quantity fish quests, the amount of the specified fish the player needs to catch. Ignored for specific fish quests
    
    /***
    For ALL QUESTS:
    *****/
    public List<CaughtFish> fishType = new List<CaughtFish>(); // For quantity fish quests, the type of fish the player needs to catch. Ignored for specific fish quests

    /*
    Rewards:
    */
    public List<Upgrade> rewards = new List<Upgrade>(); //only a list just in case

    public int moneyReward = 0; //if this is 0 it just wont do anything

    /*
    For linking to next quest
    */
    public Quest nextQuest; // If this is not null, it will be given to the player after they complete this quest
    
}
