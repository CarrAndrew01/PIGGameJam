using TMPro;

public class QuestListItem : ListItem
{
    public TextMeshProUGUI completedField;

    public QuestMenu parentMenu;

    public bool isCompletable; //whether we have the stuff in our inventory to do it. We do it here so that the button can look different if we dont have the fish

    public Quest quest;

    public void Init(QuestMenu parent, Quest quest, bool completed = false)
    {
        // QuestListItem specific initialization
        this.quest = quest;
        parentMenu = parent;
        completedField.text = completed ? "Completed" : ""; //display nothing if its not completed
        completedField.gameObject.SetActive(completed);

        // Call the usual base init
        base.Init(parent, quest.questName);
    }

    public override void OnItemClicked()
    {
        if (parentMenu == null) return;

        parentMenu.FillInField(quest);

        parentMenu.currentlyDisplayedQuestItem = this;

        // Update selection visuals for this item and siblings
        UpdateSelectionHighlight();
    }
}
