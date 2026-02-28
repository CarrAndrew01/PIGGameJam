using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

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
        if (bait.HasValue) return bait.Value.baitUpgrade.name;
        else if (fish.HasValue) return fish.Value.fish.name;
        else if (upgrade.HasValue) return upgrade.Value.upgrade.name;
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
public class ShopMenu : MonoBehaviour
{
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field

    public TextMeshProUGUI priceFieldBuy;
    public TextMeshProUGUI moneyField;
    public GameObject buyButton;


    public Sprite open;
    public Sprite closed;

    public GameObject leftButton;
    public GameObject rightButton;


    public RectTransform listContentArea; // Reference to the RectTransform for the list
    public GameObject listFishPrefab; // Prefab for the list items in the menu

    public GameObject listBaitPrefab; // Prefab for the list items in the menu
    public GameObject listUpgradePrefab; // optional prefab for upgrades (can be null)

    // Selection state now uses ShopItem
    public ShopItem selectedShopItem;

    // Source lists that will be combined into shopItems on Start
    public List<Bait> purchasableBait = new();
    public List<CaughtFish> purchasableFish = new();
    public List<PurchasableUpgrade> purchasableUpgrades = new();

    // Combined list used for populating the buy UI
    private List<ShopItem> shopItems = new();

    public void Start()
    {
        // Combine the three source lists into the unified shopItems list,
        // then open the buy view.
        CombineShopItems();
        BuySwitch();
        UpdateMoneyDisplay();

        // For testing, add some fish to the inventory
        // GameManager.Instance.playerInventory.caughtFish.Add(
        //     new CaughtFish(GameManager.Instance.TEMPFISH, 1f, "Earth"));
        // GameManager.Instance.playerInventory.caughtFish.Add(
        //     new CaughtFish(GameManager.Instance.TEMPFISH, 2f, "Water"));
        // GameManager.Instance.playerInventory.caughtFish.Add(
        //     new CaughtFish(GameManager.Instance.TEMPFISH, 3f, "Fire"));
        // GameManager.Instance.playerInventory.caughtFish.Add(
        //     new CaughtFish(GameManager.Instance.TEMPFISH, 4f, "Air"));

    }

    public void BuySwitch()
    {
        leftButton.GetComponent<Image>().sprite = open;
        rightButton.GetComponent<Image>().sprite = closed;

        mechanicalDescriptionField.gameObject.SetActive(true);
        priceFieldBuy.gameObject.SetActive(true);
        buyButton.SetActive(true);

        PopulateListWithPurchasables();
    }

    public void SellSwitch()
    {
        leftButton.GetComponent<Image>().sprite = closed;
        rightButton.GetComponent<Image>().sprite = open;

        mechanicalDescriptionField.gameObject.SetActive(false);
        priceFieldBuy.gameObject.SetActive(false);
        buyButton.SetActive(false);

        PopulateListWithFish(GameManager.Instance.playerInventory.caughtFish);
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
                        GameManager.Instance.playerInventory.AddBait(bait.baitUpgrade, 1);
                    }
                }
                UpdateMoneyDisplay();
                break;

            case ShopItem.ShopItemType.Upgrade:
                    if (item.TryGetUpgrade(out var purchUp) && purchUp.upgrade != null)
                {
                        float price = item.price;
                        if (GameManager.Money >= price)
                        {
                            GameManager.AdjustMoney(-price);
                            GameManager.AddUpgrade(purchUp.upgrade);
                        }
                }
                UpdateMoneyDisplay();
                break;

            case ShopItem.ShopItemType.Fish:
                if (item.TryGetFish(out var fish))
                {
                    float price = item.price;
                    if (GameManager.Money >= price)
                    {
                        GameManager.AdjustMoney(-price);
                        GameManager.AddFishToInventory(fish);
                    }
                }
                UpdateMoneyDisplay();
                break;
        }
    }

    public void UpdateMoneyDisplay()
    {
        moneyField.text = $"${GameManager.Money:F2}";
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
        // Reset main selection panel and clear existing list items
        selectedShopItem = null;
        nameField.text = "";
        descriptionField.text = "";
        mechanicalDescriptionField.text = "";
        // Hide description/mechanical until an item is selected
        descriptionField.gameObject.SetActive(false);
        mechanicalDescriptionField.gameObject.SetActive(false);
        // Hide buy controls until selection
        priceFieldBuy.gameObject.SetActive(false);
        buyButton.SetActive(false);

        // Clear existing list items (iterate backwards to avoid skipping when destroying)
        for (int i = listContentArea.childCount - 1; i >= 0; i--)
        {
            Destroy(listContentArea.GetChild(i).gameObject);
        }

        // Instantiate UI entries from the combined shopItems list. For types we don't have
        // a dedicated prefab for, we try to reuse available prefabs where possible.
        foreach (var item in shopItems)
        {
            if (item.Type == ShopItem.ShopItemType.Bait && item.TryGetBait(out var bait))
            {
                GameObject newItem = Instantiate(listBaitPrefab, listContentArea);
                BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
                listItemComponent.Init(bait, this, bait.baitUpgrade.name, item.price);
            }
            else if (item.Type == ShopItem.ShopItemType.Fish && item.TryGetFish(out var fish))
            {
                // Use the buy-style prefab for purchasable fish so it doesn't include sell controls
                GameObject newItem = Instantiate(listBaitPrefab, listContentArea);
                BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
                if (listItemComponent != null)
                {
                    listItemComponent.Init(fish, this, fish.fish.name, item.price);
                }
            }
            else if (item.Type == ShopItem.ShopItemType.Upgrade && item.TryGetUpgrade(out var upgrade))
            {
                // If an upgrade prefab is provided, use it; otherwise reuse the bait prefab and the new Upgrade Init overload
                GameObject newItem = null;
                if (listUpgradePrefab != null)
                    newItem = Instantiate(listUpgradePrefab, listContentArea);
                else
                    newItem = Instantiate(listBaitPrefab, listContentArea);

                BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
                if (listItemComponent != null)
                {
                    // 'upgrade' is a PurchasableUpgrade wrapper; pass the wrapper so the list item has both upgrade and price
                    listItemComponent.Init(upgrade, this, upgrade.upgrade.name, item.price);
                }
            }
        }
    }


    public void PopulateListWithFish(List<CaughtFish> fishTypes)
    {
        // Clear existing list items (iterate backwards to avoid skipping when destroying)
        for (int i = listContentArea.childCount - 1; i >= 0; i--)
        {
            Destroy(listContentArea.GetChild(i).gameObject);
        }

        // Instantiate new list items based on the provided list of caught fish
        for (int i = 0; i < GameManager.Instance.playerInventory.caughtFish.Count; i++)
        {
            CaughtFish caughtFish = GameManager.Instance.playerInventory.caughtFish[i];
            //CreateListItem(caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {(caughtFish.weight * 10):F2}", description: caughtFish.fish.description);

            GameObject newItem = Instantiate(listFishPrefab, listContentArea);
            SellListItem listItemComponent = newItem.GetComponent<SellListItem>();


            listItemComponent.Init(caughtFish, this, caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}",
            subtext2: $"Value: {GameManager.CalculateFishValue(caughtFish):F2}", description: caughtFish.fish.description);
        }
    }
}
