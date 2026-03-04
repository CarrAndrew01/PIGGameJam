using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;

[Serializable]
public struct PurchasableUpgrade
{
    public Upgrade upgrade;
    public float price;

    public PurchasableUpgrade(Upgrade upgrade, float price)
    {
        this.upgrade = upgrade;
        this.price = price;
    }
}

/// <summary>
/// Represents an item in the shop -- either a bait, a fish, or an upgrade.
/// </summary>
[Serializable]
public class ShopItem
{
    public enum ShopItemType { None, Bait, Fish, Upgrade }

    public ShopItemType Type =>
        bait.HasValue ? ShopItemType.Bait :
        fish.HasValue ? ShopItemType.Fish :
        upgrade.HasValue ? ShopItemType.Upgrade : ShopItemType.None;

    public bool TryGetBait(out Bait b) { if (bait.HasValue) { b = bait.Value; return true; } b = default; return false; }
    public bool TryGetFish(out CaughtFish f) { if (fish.HasValue) { f = fish.Value; return true; } f = default; return false; }
    public bool TryGetUpgrade(out PurchasableUpgrade u) { if (upgrade.HasValue) { u = upgrade.Value; return true; } u = default; return false; }

    // These three variables are optional -- only one should be filled
    private Bait? bait = null;
    private CaughtFish? fish = null;
    private PurchasableUpgrade? upgrade = null;

    public float price;

    public ShopItem(Bait bait, float price)
    {
        this.bait = bait;
        this.price = price;
    }
    public ShopItem(CaughtFish fish, float price)
    {
        this.fish = fish;
        this.price = price;
    }
    public ShopItem(PurchasableUpgrade upgrade, float price)
    {
        this.upgrade = upgrade;
        this.price = price;
    }

    public string GetName()
    {
        if (bait.HasValue) return bait.Value.baitUpgrade.upgradeName;
        else if (fish.HasValue) return fish.Value.fish.fishName;
        else if (upgrade.HasValue) return upgrade.Value.upgrade.upgradeName;
        else return "ERROR: No item";
    }
    public string GetDescription()
    {
        if (bait.HasValue) return bait.Value.baitUpgrade.description;
        else if (fish.HasValue) return fish.Value.fish.description;
        else if (upgrade.HasValue) return upgrade.Value.upgrade.description;
        else return "ERROR: No item";
    }
    public Sprite GetSprite()
    {
        if (bait.HasValue) return bait.Value.baitUpgrade.icon;
        else if (fish.HasValue) return fish.Value.fish.sprite;
        else if (upgrade.HasValue) return upgrade.Value.upgrade.icon;
        else return null;
    }
    public float GetPrice()
    {
        return price;
    }
}

/// <summary>
/// Handles the shop menu.
/// </summary>
public class ShopMenu : Menu
{
    public TextMeshProUGUI priceFieldBuy;
    public GameObject buyButton;


    public Sprite open;
    public Sprite closed;

    public GameObject leftButton;
    public GameObject rightButton;


    public GameObject listSellPrefab; // Prefab for the list items in the menu

    public GameObject listBuyPrefab; // Prefab for the list items in the menu
    public GameObject dividerPrefab; // optional prefab for dividers (can be null)

    // Selection state now uses ShopItem
    public ShopItem selectedShopItem;

    // Source lists that will be combined into shopItems on Start
    public List<Bait> purchasableBait = new();
    public List<CaughtFish> purchasableFish = new();
    public List<PurchasableUpgrade> purchasableUpgrades = new();

    // Combined list used for populating the buy UI
    private List<ShopItem> shopItems = new();

    
    [Header("Input Actions")]
    public InputActionReference previousPageAction; // expects Button
    public InputActionReference nextPageAction; // expects Button

    public void Start()
    {
        // Combine the three source lists into the unified shopItems list,
        // then open the buy view.
        CombineShopItems();
        BuySwitch();
        UpdateMoneyDisplay();
    }

    public override void Update()
    {
        base.Update();

        // Page navigation (if applicable)
        if (previousPageAction != null && previousPageAction.action.WasPressedThisFrame())
        {
            BuySwitch();
        }
        else if (nextPageAction != null && nextPageAction.action.WasPressedThisFrame())
        {
            SellSwitch();
        }
    }

    public override void OnListItemSubmitted(ListItem item)
    {
        if (item is SellListItem sell)
            sell.OnSellClicked();
        else if (item is BuyListItem)
            BuyButton();
    }

    public void BuySwitch()
    {
        leftButton.GetComponent<Image>().sprite = open;
        rightButton.GetComponent<Image>().sprite = closed;

        nameField.gameObject.SetActive(true);
        descriptionField.gameObject.SetActive(true);
        mechanicalDescriptionField.gameObject.SetActive(true);

        // Buy controls on for buying
        priceFieldBuy.transform.parent.gameObject.SetActive(true);
        priceFieldBuy.gameObject.SetActive(true);
        buyButton.SetActive(true);

        PopulateListWithPurchasables();
    }

    public void SellSwitch()
    {
        leftButton.GetComponent<Image>().sprite = closed;
        rightButton.GetComponent<Image>().sprite = open;

        nameField.gameObject.SetActive(true);
        descriptionField.gameObject.SetActive(true);
        mechanicalDescriptionField.gameObject.SetActive(false);

        // Buy controls off for selling
        priceFieldBuy.transform.parent.gameObject.SetActive(false);
        priceFieldBuy.gameObject.SetActive(false);
        buyButton.SetActive(false);

        PopulateSellList(GameManager.Instance.playerInventory.caughtFish);
    }

    public void BuyButton()
    {
        var item = selectedShopItem;
        if (item == null) return;

        switch (item.Type)
        {
            case ShopItem.ShopItemType.Bait:
                if (item.TryGetBait(out var bait))
                {
                    float price = item.price;
                    if (GameManager.Money >= price)
                    {
                        GameManager.AdjustMoney(-price);
                        GameManager.Instance.playerInventory.AddBait(bait.baitUpgrade, bait.numberOfUses);
                        Toast.ShowToast($"Purchased #{bait.numberOfUses} {bait.baitUpgrade.name} for ${price:F2}!", icon: bait.baitUpgrade.icon);
                    }
                    else
                    {
                        Toast.ShowToast($"Cannot purchase {bait.baitUpgrade.name}: Not enough money!");
                    }
                }
                UpdateMoneyDisplay();
                break;

            case ShopItem.ShopItemType.Upgrade:
                if (item.TryGetUpgrade(out var purchUp) && purchUp.upgrade != null)
                {
                    float price = item.price;
                    if (GameManager.Money >= price && !GameManager.PlayerHasUpgrade(purchUp.upgrade)) // Prevent buying the same upgrade multiple times
                    {
                        GameManager.AdjustMoney(-price);
                        GameManager.AddUpgrade(purchUp.upgrade);
                        Toast.ShowToast($"Purchased {purchUp.upgrade.upgradeName} for ${price:F2}!", icon: purchUp.upgrade.icon);

                        // stamp the matching UI item so it shows purchased immediately
                        foreach (Transform child in listContentArea)
                        {
                            var buyItem = child.GetComponent<BuyListItem>();
                            if (buyItem != null && buyItem.upgradeRef.upgrade == purchUp.upgrade)
                            {
                                buyItem.Stamp();
                                break;
                            }
                        }
                    }
                    else if (GameManager.PlayerHasUpgrade(purchUp.upgrade))
                    {
                        Toast.ShowToast($"Cannot purchase {purchUp.upgrade.upgradeName}: Upgrade already owned!");
                    }
                    else
                    {
                        Toast.ShowToast($"Cannot purchase {purchUp.upgrade.upgradeName}: Not enough money!");
                    }
                }
                UpdateMoneyDisplay();
                break;

            case ShopItem.ShopItemType.Fish:
                if (item.TryGetFish(out var fish))
                {
                    float price = item.price;
                    if (GameManager.Money >= price && !GameManager.IsInventoryFull())
                    {
                        GameManager.AdjustMoney(-price);
                        GameManager.AddFishToInventory(fish);
                        Toast.ShowToast($"Purchased {fish.fish.fishName} for ${price:F2}!", icon: fish.fish.sprite);
                    }
                    else if (GameManager.IsInventoryFull())
                    {
                        Toast.ShowToast($"Cannot purchase {fish.fish.fishName}: Inventory is full!");
                    }
                    else
                    {
                        Toast.ShowToast($"Cannot purchase {fish.fish.fishName}: Not enough money!");
                    }
                }
                UpdateMoneyDisplay();
                break;
        }
    }

    private void CombineShopItems()
    {
        shopItems.Clear();

        foreach (var b in purchasableBait)
        {
            shopItems.Add(new ShopItem(b, b.cost));
        }

        foreach (var f in purchasableFish)
        {
            // Use the calculated fish value as the purchase price by default
            shopItems.Add(new ShopItem(f, GameManager.CalculateFishValue(f)));
        }

        foreach (var u in purchasableUpgrades)
        {
            // Use the wrapper's price so upgrades use their configured price
            shopItems.Add(new ShopItem(u, u.price));
        }
    }

    public void PopulateListWithPurchasables()
    {
        BeginListRebuild();
        // Reset main selection panel and clear existing list items
        selectedShopItem = null;
        nameField.text = "Buying Items";
        descriptionField.text = "";
        mechanicalDescriptionField.text = "Click on an item to see its description.";
        // Hide buy controls until selection
        priceFieldBuy.transform.parent.gameObject.SetActive(false);
        priceFieldBuy.gameObject.SetActive(false);
        buyButton.SetActive(false);

        // Clear existing list items (iterate backwards to avoid skipping when destroying)
        listItems.Clear();
        for (int i = listContentArea.childCount - 1; i >= 0; i--)
        {
            Destroy(listContentArea.GetChild(i).gameObject);
        }

        // Instantiate dividers between item types
        ShopItem.ShopItemType lastType = ShopItem.ShopItemType.None;
        foreach (var item in shopItems)
        {
            var type = item.Type;

            // If we hit a new type, insert a divider (if provided) with the appropriate label
            if (type != lastType)
            {
                if (dividerPrefab != null)
                {
                    GameObject divider = Instantiate(dividerPrefab, listContentArea);
                    TextMeshProUGUI label = divider.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null)
                    {
                        switch (type)
                        {
                            case ShopItem.ShopItemType.Bait:
                                label.text = "Bait";
                                break;
                            case ShopItem.ShopItemType.Fish:
                                label.text = "Fish";
                                break;
                            case ShopItem.ShopItemType.Upgrade:
                                label.text = "Upgrades";
                                break;
                            default:
                                label.text = "";
                                break;
                        }
                    }
                }
                lastType = type;
            }

            if (type == ShopItem.ShopItemType.Bait && item.TryGetBait(out var bait))
            {
                GameObject newItem = Instantiate(listBuyPrefab, listContentArea);
                BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
                if (listItemComponent != null)
                {
                    listItemComponent.Init(bait, this, bait.baitUpgrade.upgradeName, item.price);
                    listItems.Add(listItemComponent);
                }
            }
            else if (type == ShopItem.ShopItemType.Fish && item.TryGetFish(out var fish))
            {
                GameObject newItem = Instantiate(listBuyPrefab, listContentArea);
                BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
                if (listItemComponent != null)
                {
                    listItemComponent.Init(fish, this, fish.fish.fishName, item.price);
                    listItems.Add(listItemComponent);
                }
            }
            else if (type == ShopItem.ShopItemType.Upgrade && item.TryGetUpgrade(out var upgrade))
            {
                GameObject newItem = Instantiate(listBuyPrefab, listContentArea);
                BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
                if (listItemComponent != null)
                {
                    listItemComponent.Init(upgrade, this, upgrade.upgrade.upgradeName, item.price);
                    listItems.Add(listItemComponent);
                }

                // If the player already owns this upgrade, visually stamp the item
                var playerUps = GameManager.GetPlayerUpgrades();
                if (playerUps != null && playerUps.Contains(upgrade.upgrade))
                {
                    listItemComponent.Stamp();
                }
            }
        }

        SetupNavigation();
    }


    public void PopulateSellList(List<CaughtFish> fishTypes)
    {
        BeginListRebuild();
        // Reset main selection panel and clear existing list items
        listItems.Clear();
        selectedShopItem = null;
        nameField.text = "Selling Fish";
        descriptionField.text = "";
        // Clear existing list items (iterate backwards to avoid skipping when destroying)
        for (int i = listContentArea.childCount - 1; i >= 0; i--)
        {
            Destroy(listContentArea.GetChild(i).gameObject);
        }

        // Instantiate new list items based on the provided list of caught fish
        for (int i = 0; i < GameManager.Instance.playerInventory.caughtFish.Count; i++)
        {
            CaughtFish caughtFish = GameManager.Instance.playerInventory.caughtFish[i];

            GameObject newItem = Instantiate(listSellPrefab, listContentArea);
            SellListItem listItemComponent = newItem.GetComponent<SellListItem>();

            listItemComponent.Init(caughtFish, this, caughtFish.fish.fishName, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}",
            subtext2: $"Value: {GameManager.CalculateFishValue(caughtFish):F2}", description: caughtFish.fish.description);
            listItems.Add(listItemComponent);
        }

        SetupNavigation();
    }
}
