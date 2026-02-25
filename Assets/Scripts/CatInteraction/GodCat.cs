using UnityEngine;

public class GodCat : CatInteract
{
    public GameObject questMenu;

    public override void InteractWithCat()
    {
        Debug.Log("1");
        base.InteractWithCat();
        // For now, just give the player 100 money when they interact with the god cat, but later this will be replaced with a dialogue sequence where the player can choose to accept the god cat's blessing or not, and if they accept it will give them a random blessing that will affect their gameplay in some way, and if they reject it nothing will happen
        TempMakeMenu();
    }

    void TempMakeMenu()
    {
        questMenu.SetActive(true);
        questMenu.GetComponentInChildren<QuestMenu>().PopulateQuestList();
    }
}
