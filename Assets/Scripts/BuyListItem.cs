using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class BuyListItem : MonoBehaviour
{
    // Components
    [Header("Data")]

    [Header("Components")]
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI price;

    public ShopMenu parentMenu;
    public Bait baitRef; //the bait that this list item is referencing
    public PurchasableUpgrade upgradeRef; // optional: if this entry represents an upgrade
    public float priceValue; // stored numeric price for this entry
    public CaughtFish fishRef; // optional: if this entry represents a fish


    public void Init(Bait fish, ShopMenu parent, string name, float price)
    {
        baitRef = fish;
        parentMenu = parent;
        nameField.text = name;
        this.price.text = $"Price: {price}";
        priceValue = price;
    }

    public void Init(PurchasableUpgrade up, ShopMenu parent, string name, float price)
    {
        upgradeRef = up;
        parentMenu = parent;
        nameField.text = name;
        this.price.text = $"Price: {price}";
        priceValue = price;
    }

    public void Init(CaughtFish fish, ShopMenu parent, string name, float price)
    {
        fishRef = fish;
        parentMenu = parent;
        nameField.text = name;
        this.price.text = $"Price: {price}";
        priceValue = price;
    }

    public void OnItemClicked()
    {
        if (parentMenu == null) return;

        // If this list item represents an upgrade, select it as such
        if (upgradeRef.upgrade != null)
        {
            parentMenu.selectedShopItem = new ShopItem(upgradeRef, priceValue);
            parentMenu.nameField.text = upgradeRef.upgrade.name;
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";
            parentMenu.mechanicalDescriptionField.text = upgradeRef.upgrade.GetMechanicalDescription();

            // Enable only the mechanical description for upgrades
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(!string.IsNullOrEmpty(parentMenu.mechanicalDescriptionField.text));
            parentMenu.descriptionField.gameObject.SetActive(false);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.buyButton.SetActive(true);
            return;
        }
        // If this entry represents a fish, select that
        if (fishRef.fish != null)
        {
            parentMenu.selectedShopItem = new ShopItem(fishRef, priceValue);
            parentMenu.nameField.text = fishRef.fish.name;
            parentMenu.descriptionField.text = fishRef.fish.description;

            // Enable only the description for fish
            parentMenu.descriptionField.gameObject.SetActive(!string.IsNullOrEmpty(parentMenu.descriptionField.text));
            parentMenu.mechanicalDescriptionField.gameObject.SetActive(false);
            parentMenu.priceFieldBuy.gameObject.SetActive(true);
            parentMenu.priceFieldBuy.text = $"Price: {priceValue}";
            parentMenu.buyButton.SetActive(true);
            return;
        }

        // Otherwise treat as bait
        parentMenu.selectedShopItem = new ShopItem(baitRef, priceValue);
        parentMenu.nameField.text = baitRef.baitUpgrade.name;
        parentMenu.mechanicalDescriptionField.text = baitRef.baitUpgrade.GetMechanicalDescription();

        // Enable only the mechanical description for bait/upgrade
        parentMenu.mechanicalDescriptionField.gameObject.SetActive(!string.IsNullOrEmpty(parentMenu.mechanicalDescriptionField.text));
        parentMenu.descriptionField.gameObject.SetActive(false);
        parentMenu.priceFieldBuy.gameObject.SetActive(true);
        parentMenu.priceFieldBuy.text = $"Price: {priceValue}";
        parentMenu.buyButton.SetActive(true);
    }
}
