using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : MonoBehaviour
{


    public TextMeshProUGUI NameField;
    public TextMeshProUGUI DescriptionField;


    public GameObject finishQuestUnavailable;
    public GameObject finishQuestAvailable;

    public GameObject questComplete;


    public RectTransform listContentArea; // Reference to the RectTransform for the list


    [Header("Prefabs")]
    public GameObject listItemPrefab; // Prefab for the list items in the menu

    public QuestListItem currentlyDisplayedQuestItem;


    public List<GameObject> requirementObjects;

    public TextMeshProUGUI rewardText;

    [Header("Cache")]
    public Quest.Completed[] cachedQuestStatuses; // Cache for quest statuses loaded from player prefs, indexed to match the order of quests in GameManager.Instance.AllQuests


    public void Start()
    {
        PopulateQuestList();
    }

    // Methods
    public void CloseMenu()
    {
        GameManager.MenuPopup.TriggerPopOut();
    }

    public void PopulateQuestList()
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        List<Quest> questDisplayed = new();

        // Grab the cached quest statuses from player prefs
        cachedQuestStatuses = CacheAllQuestStatuses(GameManager.Instance.AllQuests);

        // Temporary lists to combine in andrew's preferred order (active quests first, then completed quests)
        List<Quest> activeQuests = new();
        List<Quest> completedQuests = new();

        // Build lists based solely on the cached statuses (do not modify the scriptable assets)
        for (int i = 0; i < GameManager.Instance.AllQuests.Count; i++)
        {
            Quest quest = GameManager.Instance.AllQuests[i];

            if (cachedQuestStatuses[i] == Quest.Completed.Active)
            {
                activeQuests.Add(quest);
            }
            else if (cachedQuestStatuses[i] == Quest.Completed.Completed)
            {
                completedQuests.Add(quest);
            }
        }

        // Combine active quests and completed quests into the final display list
        questDisplayed.AddRange(activeQuests);
        questDisplayed.AddRange(completedQuests);

        for (int i = 0; i < questDisplayed.Count; i++)
        {
            Quest quest = questDisplayed[i];

            int origIndex = GameManager.Instance.AllQuests.IndexOf(quest);
            bool isCompleted = origIndex >= 0 && cachedQuestStatuses[origIndex] == Quest.Completed.Completed;

            QuestListItem created = CreateListItem(quest, isCompleted);
            if (i == 0 && created != null)
            {
                currentlyDisplayedQuestItem = created;
            }
        }

        if (questDisplayed.Count > 0)
        {
            FillInField(questDisplayed[0]);
        }
        else
        {
            NameField.text = "No Quests";
            DescriptionField.text = "Who knows why.";
            rewardText.transform.parent.gameObject.SetActive(false); //turn off the reward text, just in case
            foreach (GameObject go in requirementObjects)
            {
                go.SetActive(false);
            }
            finishQuestAvailable.SetActive(false);
            finishQuestUnavailable.SetActive(false);
            questComplete.SetActive(false);
        }
    }

    /**
    for filling in the info on the main panel
    */
    public void FillInField(Quest quest)
    {

        if (quest.questFish.Count <= 0)
        {
            return;
        }

        Quest.Completed displayStatus = GetCachedStatusForQuest(quest);

        NameField.text = quest.questName;
        DescriptionField.text = quest.description;
        //depending on what requirements are needed

        GameObject go = requirementObjects[quest.questFish.Count - 1];

        go.SetActive(true); //turn on the correct number of requirement objects, we have 3 so if its 2 we want to turn on index 0 and 1, if its 3 we want to turn on all of them, etc

        TextMeshProUGUI[] labels = go.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = go.GetComponentsInChildren<Image>();


        for (int i = 0; i < quest.questFish.Count; i++)
        {
            labels[i].text = $"{quest.questFish[i].quantity} X {quest.questFish[i].fishType.name}";
            images[i].sprite = quest.questFish[i].fishType.sprite;
        }

        rewardText.transform.parent.gameObject.SetActive(true); //turn on the reward text, just in case it was turned off for a quest with no reward

        if (quest.moneyReward > 0)
        {
            rewardText.text = $"Money: {quest.moneyReward} ";
        }
        else
        {
            rewardText.text = quest.rewardUpgrade.name;
        }



        // Use cached/player-prefs status for UI decisions
        if (displayStatus == Quest.Completed.Active)
        {
            if (CheckCompletion(quest))
            {
                finishQuestAvailable.SetActive(true);
                finishQuestUnavailable.SetActive(false);
            }
            else
            {
                finishQuestAvailable.SetActive(false);
                finishQuestUnavailable.SetActive(true);
            }
            questComplete.SetActive(false); //hidden dont show up so just do this

        }
        else
        {
            Debug.Log("123");
            finishQuestAvailable.SetActive(false);
            finishQuestUnavailable.SetActive(false);
            questComplete.SetActive(true); //hidden dont show up so just do this
        }
    }

    // Helper to read the status for a quest from the cache (or fall back to the asset default)
    private Quest.Completed GetCachedStatusForQuest(Quest quest)
    {
        if (cachedQuestStatuses == null || GameManager.Instance == null)
            return quest.completedStatus;

        int idx = GameManager.Instance.AllQuests.IndexOf(quest);
        if (idx < 0 || idx >= cachedQuestStatuses.Length)
            return quest.completedStatus;

        return cachedQuestStatuses[idx];
    }

    private QuestListItem CreateListItem(Quest quest, bool completed)
    {
        GameObject newItem = Instantiate(listItemPrefab, listContentArea);
        QuestListItem listItemComponent = newItem.GetComponent<QuestListItem>();

        // Set the list item data
        if (listItemComponent != null)
        {
            listItemComponent.Init(this, quest, completed);
            return listItemComponent;
        }
        else
        {
            Debug.LogError("List item prefab is missing a ListItem component!");
            return null;
        }
    }

    //check if the palyer has the requirements for a quest. This works for general quantity quests AND special fish quest
    public static bool CheckCompletion(Quest quest)
    {
        //check if the player has caught enough of the required fish
        foreach (QuestSubtype fish in quest.questFish)
        {
            if (!(GameManager.Instance.playerInventory.NumberOfFish(fish.fishType.name) >= fish.quantity))
            {
                return false;
            }
        }
        return true;
    }


    /*
        handles logic for quest completion, and updates stuff
    */
    public void OnQuestCompletion()
    {
        finishQuestAvailable.SetActive(false); //hide the button, just in case
        questComplete.SetActive(true); //show the quest complete text

        currentlyDisplayedQuestItem.completedField.text = "Completed"; //update the text on the list item, just in case

        //this all seems terrible, there's certainly a better way to do this, maybe by using direct comparison instead of strings for names
        //but I'm not sure if thats safe so I'm just gonna use strings
        // Find and mark the completed quest in-memory and in prefs
        string completedName = currentlyDisplayedQuestItem.quest.questName;
        for (int i = 0; i < GameManager.Instance.AllQuests.Count; i++)
        {
            Quest q = GameManager.Instance.AllQuests[i];
            if (q.questName == completedName)
            {
                // Do NOT modify the scriptable asset. Update the cache + prefs instead.
                if (cachedQuestStatuses != null && i < cachedQuestStatuses.Length)
                    cachedQuestStatuses[i] = Quest.Completed.Completed;
                SaveQuestStatus(q.questName, Quest.Completed.Completed);

                // remove the fish from our inventory for this quest
                foreach (QuestSubtype fish in q.questFish)
                {
                    GameManager.Instance.playerInventory.RemoveFishQuest(fish.fishType.name, fish.quantity);
                }
                break;
            }
        }

        // Unlock next quests listed on the completed quest
        foreach (string nextQuestName in currentlyDisplayedQuestItem.quest.nextQuest)
        {
            for (int j = 0; j < GameManager.Instance.AllQuests.Count; j++)
            {
                Quest nq = GameManager.Instance.AllQuests[j];
                if (nq.questName == nextQuestName)
                {
                    // activate via cache + prefs only
                    if (cachedQuestStatuses != null && j < cachedQuestStatuses.Length)
                        cachedQuestStatuses[j] = Quest.Completed.Active;
                    SaveQuestStatus(nq.questName, Quest.Completed.Active);
                }
            }
        }

        //give the player the reward

        if (currentlyDisplayedQuestItem.quest.moneyReward > 0)
        {
            GameManager.AdjustMoney(currentlyDisplayedQuestItem.quest.moneyReward);
        }
        else
        {
            GameManager.AddUpgrade(currentlyDisplayedQuestItem.quest.rewardUpgrade);
        }
    }

    // NOTE: Added these to store completion data into player prefs -- it will persist across sessions unless you reset it
    public static void SaveQuestStatus(string questName, Quest.Completed status)
    {
        PlayerPrefs.SetInt($"Quest_{questName}_Status", (int)status);
        PlayerPrefs.Save();
    }
    public static Quest.Completed LoadQuestStatus(string questName)
    {
        int statusInt = PlayerPrefs.GetInt($"Quest_{questName}_Status", (int)Quest.Completed.Hidden);
        return (Quest.Completed)statusInt;
    }
    public static Quest.Completed[] CacheAllQuestStatuses(List<Quest> quests)
    {
        Quest.Completed[] statuses = new Quest.Completed[quests.Count];
        for (int i = 0; i < quests.Count; i++)
        {
            string key = $"Quest_{quests[i].questName}_Status";
            if (PlayerPrefs.HasKey(key))
            {
                statuses[i] = LoadQuestStatus(quests[i].questName);
            }
            else
            {
                // If there's no saved value yet, keep the scriptable asset's default status
                statuses[i] = quests[i].completedStatus;
            }
        }
        return statuses;
    }
}
