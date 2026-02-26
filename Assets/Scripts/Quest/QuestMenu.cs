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

        // Instantiate new list items based on the provided array of strings
        foreach (Quest quest in GameManager.Instance.AllQuests)
        {
            if (quest.completedStatus != Quest.Completed.Active)
            {
                continue;
            }

            questDisplayed.Add(quest);
            CreateListItem(quest, false);
        }

        // Instantiate new list items based on the provided array of strings
        foreach (Quest quest in GameManager.Instance.AllQuests)
        {
            if (quest.completedStatus != Quest.Completed.Completed)
            {
                continue;
            }
            questDisplayed.Add(quest);
            CreateListItem(quest, true);
        }

        FillInField(questDisplayed[0]);
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



        //no no this is actually the best way to do this actually seriously dude
        if (quest.completedStatus == Quest.Completed.Active)
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

    private void CreateListItem(Quest quest, bool completed)
    {
        GameObject newItem = Instantiate(listItemPrefab, listContentArea);
        QuestListItem listItemComponent = newItem.GetComponent<QuestListItem>();

        // Set the list item data
        if (listItemComponent != null)
        {
            listItemComponent.Init(this, quest, completed);
        }
        else
        {
            Debug.LogError("List item prefab is missing a ListItem component!");
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
        foreach (Quest q in GameManager.Instance.AllQuests)
        {
            if (q.questName == currentlyDisplayedQuestItem.quest.questName)
            {
                q.completedStatus = Quest.Completed.Completed;

                //now we need to remove the fish from our inventory
                foreach (QuestSubtype fish in q.questFish)
                {
                    GameManager.Instance.playerInventory.RemoveFishQuest(fish.fishType.name, fish.quantity);
                }
            }

            //unlock the next quests
            foreach (string nextQuest in q.nextQuest)
            {
                if (q.questName == nextQuest)
                {
                    q.completedStatus = Quest.Completed.Active;
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
}
