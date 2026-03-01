using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class BuyListItem : ListItem
{
    // Components
    [Header("Buy List Components")]
    [ShowInInspector, ReadOnly] public Bait baitRef; //the bait that this list item is referencing
    [ShowInInspector, ReadOnly] public PurchasableUpgrade upgradeRef; // optional: if this entry represents an upgrade
    [ShowInInspector, ReadOnly] public CaughtFish fishRef; // optional: if this entry represents a fish
    [ShowInInspector, ReadOnly] public float priceValue; // stored numeric price for this entry

    [ShowInInspector, ReadOnly] private ShopMenu parentMenu;


    public void Init(Bait bait, ShopMenu parent, string name, float price)
    {
        baitRef = bait;
        parentMenu = parent;
        nameField.text = name;
        priceValue = price;

        SetupComponents(null, $"Price: {price:F2}");
    }

    public void Init(PurchasableUpgrade up, ShopMenu parent, string name, float price)
    {
        upgradeRef = up;
        parentMenu = parent;
        nameField.text = name;
        priceValue = price;

        SetupComponents(up.upgrade.icon, $"Price: {price:F2}");
    }

    public void Init(CaughtFish fish, ShopMenu parent, string name, float price)
    {
        fishRef = fish;
        parentMenu = parent;
        nameField.text = name;
        priceValue = price;

        SetupComponents(fish.fish.sprite, $"Price: {price:F2}");
    }

    public override void OnItemClicked()
    {
        if (parentMenu == null) return;

        if (upgradeRef.upgrade != null)
        { // If this list item represents an upgrade, select it as such
            parentMenu.selectedShopItem = new ShopItem(upgradeRef, priceValue);
            parentMenu.nameField.text = upgradeRef.upgrade.upgradeName;
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";
            parentMenu.mechanicalDescriptionField.text = upgradeRef.upgrade.GetMechanicalDescription();

            // Enable only the mechanical description for upgrades
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(!string.IsNullOrEmpty(parentMenu.mechanicalDescriptionField.text));
            parentMenu.descriptionField.gameObject.SetActive(false);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.buyButton.SetActive(true);

            UpdateSelectionHighlight();
        }
        else if (fishRef.fish != null)
        { // If this entry represents a fish, select that
            parentMenu.selectedShopItem = new ShopItem(fishRef, priceValue);
            parentMenu.nameField.text = fishRef.fish.fishName;
            parentMenu.descriptionField.text = fishRef.fish.description;
            parentMenu.mechanicalDescriptionField.text = "It's a fish!";

            // Enable only the description for fish
            parentMenu.descriptionField.gameObject.SetActive(!string.IsNullOrEmpty(parentMenu.descriptionField.text));
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(false);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";
            parentMenu.buyButton.SetActive(true);

            UpdateSelectionHighlight();
        }
        else
        { // Otherwise treat as bait
            parentMenu.selectedShopItem = new ShopItem(baitRef, priceValue);
            parentMenu.nameField.text = baitRef.baitUpgrade.upgradeName;
            parentMenu.mechanicalDescriptionField.text = baitRef.baitUpgrade.GetMechanicalDescription();

            // Enable only the mechanical description for bait/upgrade
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(!string.IsNullOrEmpty(parentMenu.mechanicalDescriptionField.text));
            parentMenu.descriptionField.gameObject.SetActive(false);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";
            parentMenu.buyButton.SetActive(true);

            // Update selection visuals for this item and siblings
            UpdateSelectionHighlight();
        }
    }
}
