using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

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

    public TextMeshProUGUI multField;  // Used to show bait quantity

    [ShowInInspector, ReadOnly] private ShopMenu parentMenu;


    public void Init(Bait bait, ShopMenu parent, string name, float price)
    {
        baitRef = bait;
        parentMenu = parent;
        parentMenuBase = parent;
        nameField.text = name;
        priceValue = price;
        multField.text = $"X{bait.numberOfUses}";
        multField.gameObject.SetActive(bait.numberOfUses > 1);

        SetupComponents(null, $"Price: {price:F2}");
    }

    public void Init(PurchasableUpgrade upgrade, ShopMenu parent, string name, float price)
    {
        upgradeRef = upgrade;
        parentMenu = parent;
        parentMenuBase = parent;
        nameField.text = name;
        priceValue = price;
        multField.gameObject.SetActive(false);

        SetupComponents(upgrade.upgrade.icon, $"Price: {price:F2}");
    }

    public void Init(CaughtFish fish, ShopMenu parent, string name, float price)
    {
        fishRef = fish;
        parentMenu = parent;
        parentMenuBase = parent;
        nameField.text = name;
        priceValue = price;
        multField.gameObject.SetActive(false);

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
            parentMenu.descriptionField.text = upgradeRef.upgrade.description;
            parentMenu.mechanicalDescriptionField.text = upgradeRef.upgrade.GetMechanicalDescription();

            // Enable the mechanical description for upgrades
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(true);

            // Buy controls on for upgrades
            parentMenu.priceFieldBuy.transform.parent.gameObject.SetActive(true);
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

            // Disable the mechanical description for fish
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(false);
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";

            // Buy controls on for fish
            parentMenu.priceFieldBuy.transform.parent.gameObject.SetActive(true);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.buyButton.SetActive(true);
            UpdateSelectionHighlight();
        }
        else
        { // Otherwise treat as bait
            parentMenu.selectedShopItem = new ShopItem(baitRef, priceValue);
            parentMenu.nameField.text = baitRef.baitUpgrade.upgradeName;
            parentMenu.descriptionField.text = baitRef.baitUpgrade.description;
            parentMenu.mechanicalDescriptionField.text = baitRef.baitUpgrade.GetMechanicalDescription();

            // Enable the mechanical description for bait/upgrade
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(true);
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";

            // Buy controls on for bait
            parentMenu.priceFieldBuy.transform.parent.gameObject.SetActive(true);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.buyButton.SetActive(true);

            // Update selection visuals for this item and siblings
            UpdateSelectionHighlight();
        }

        parentMenuBase?.OnListItemSelected(listIndex);
    }
}
