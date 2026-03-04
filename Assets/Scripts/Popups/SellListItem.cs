using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class SellListItem : ListItem
{
    // Components
    [Header("Sell List Components")]
    [ShowInInspector, ReadOnly] public CaughtFish fishRef; //the fish that this list item is referencing

    [ShowInInspector, ReadOnly] private ShopMenu parentMenu;

    public void Init(CaughtFish fish, ShopMenu parent, string name, Sprite iconSprite = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "")
    {
        fishRef = fish;
        parentMenu = parent;
        base.Init(parent, name, iconSprite, subtext, subtext2, description, mechanicalDescription);
    }

    public void OnSellClicked()
    {
        // Remove from inventory and add money
        GameManager.Instance.playerInventory.RemoveFishByID(fishRef.id);
        GameManager.AdjustMoney(GameManager.CalculateFishValue(fishRef));

        // Refresh the parent menu to ensure UI matches inventory (safer than relying on Destroy timing)
        if (parentMenu != null)
        {
            parentMenu.PopulateSellList(GameManager.Instance.playerInventory.caughtFish);
            parentMenu.UpdateMoneyDisplay();
        }
    }

    public override void OnItemClicked()
    {
        if (parentMenu != null)
        {
            parentMenu.nameField.text = fishRef.fish.fishName;
            parentMenu.descriptionField.text = description;
            parentMenu.mechanicalDescriptionField.text = mechanicalDescription;
        }

        // Update selection visuals for this item and siblings
        UpdateSelectionHighlight();
    }
}
