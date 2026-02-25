using System.Collections.Generic;
using UnityEngine;


//WHY
//AREN"T
//FICTIONARYS
//FUCKING
//SERIALIZABLE
//AAAAAAAAAHHHHHHHHHH
[System.Serializable]
public class QuestSubtype
{
    public Fish fishType;
    public int quantity;

}

[System.Serializable]
public class Quest
{
    public string questName = "";
    public string description = "";
    
    //I will have this here for conveniance, but we also have a seperate list of completed and active quests in GameManager. It's kind of doubling up,
    //but it makes the code cleaner for populating
    public enum Completed
    {
        Active,
        Completed,
        Hidden

    }

    public Completed completedStatus = Completed.Hidden;

    public enum QuestType
    {
        QuantityFish, // Catch a certain type of fish
        SpecificFish, //catch a rare variant fish
    }

    public QuestType type;


    /***
    For ALL QUESTS:
    *****/
    public List<QuestSubtype> questFish = new List<QuestSubtype>(); // For all quests, you need to provide a certain type of fish
    //for unique fish (i.e. special or "unique" fish) the fish is just saved the same as a different species


    /*
    Rewards:
    */
    public Upgrade rewardUpgrade; //only a list just in case

    public int moneyReward = 0; //if this is 0 it just wont do anything

    /*
    For linking to next quest
    */
    public List<string> nextQuest; // If this is not null, it will be given to the player after they complete this quest    
    //its easier to keep string refs instead of a ref to the Quest 
}
