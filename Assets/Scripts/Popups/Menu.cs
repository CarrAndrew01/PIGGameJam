using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Generic menu class used for various menus with a list of items.
/// </summary>
public class Menu : MonoBehaviour
{
    // State
    public List<ListItem> listItems = new List<ListItem>(); // List of the current list items in the menu
    public int selectedIndex = -1; // Index of the currently selected item, -1 if none selected

    [Header("Events")]
    public UnityEvent<int> onItemSelected; // Event that gets triggered when an item is selected, passing the index of the selected item

    // Components
    [Header("Components")]
    public RectTransform listContentArea; // Reference to the RectTransform for the list
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field
    public TextMeshProUGUI moneyField;

    [Header("Prefabs")]
    public GameObject listItemPrefab; // Prefab for the list items in the menu

    void Awake()
    {
        descriptionField.text = "";
        mechanicalDescriptionField.text = "Click on an item to see its description.";
        UpdateMoneyDisplay();
    }

    // Methods
    public void OnListItemSelected(int index)
    {
        selectedIndex = index;
        onItemSelected?.Invoke(index);

        // Update selection highlight for all list items using the new SetSelected API
        for (int i = 0; i < listItems.Count; i++)
        {
            listItems[i].SetSelected(i == selectedIndex);
        }
    }

    public void PopulateList(string[] items)
    {
        // Clear existing list items
        listItems.Clear();
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided array of strings
        foreach (string item in items)
        {
            CreateListItem(item, null);
        }

        SetupNavigation();
    }

    public void UpdateMoneyDisplay()
    {
        if (moneyField != null) moneyField.text = $"${GameManager.Money:F2}";
    }

    public void PopulateListWithUpgrades(List<Upgrade> upgrades)
    {
        // Clear existing list items
        listItems.Clear();
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided list of upgrades
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade upgrade = upgrades[i];
            CreateListItem(upgrade.upgradeName, upgrade.icon, description: upgrade.description, mechanicalDescription: upgrade.GetMechanicalDescription(), index: i);
        }
        SetupNavigation();
    }
    public void PopulateListWithBaits(List<Bait> baits)
    {
        // Clear existing list items
        listItems.Clear();
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided list of baits
        for (int i = 0; i < baits.Count; i++)
        {
            Bait bait = baits[i];
            CreateListItem(bait.baitUpgrade.upgradeName, bait.baitUpgrade.icon, subtext: $"Uses: {bait.numberOfUses}", description: bait.baitUpgrade.description, mechanicalDescription: bait.baitUpgrade.GetMechanicalDescription(), index: i);
        }
        SetupNavigation();
    }
    public void PopulateListWithFish(List<CaughtFish> fishTypes)
    {
        // Clear existing list items
        listItems.Clear();
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided list of caught fish
        for (int i = 0; i < fishTypes.Count; i++)
        {
            CaughtFish caughtFish = fishTypes[i];
            CreateListItem(caughtFish.fish.fishName, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {GameManager.CalculateFishValue(caughtFish):F2}", description: caughtFish.fish.description, mechanicalDescription: $"Planet of origin: {caughtFish.planetOfOrigin}", index: i);
        }
        SetupNavigation();
    }

    public void PopulateListWithFishCount(Dictionary<string, int> fishCount, List<CaughtFish> fishTypes = null)
    {
        // Clear existing list items
        listItems.Clear();
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided dictionary of strings and ints
        foreach (KeyValuePair<string, int> fish in fishCount)
        {
            // Find the corresponding fish type for the item
            Sprite itemIcon = null;
            string itemDescription = "";
            string itemMechanicalDescription = "";
            string displayName = fish.Key;
            if (fishTypes != null)
            {
                CaughtFish caughtFish = fishTypes.Find(f => f.fish.name == fish.Key);
                itemIcon = caughtFish.fish.sprite;
                itemDescription = caughtFish.fish.description;
                itemMechanicalDescription = $"Planet of origin: {caughtFish.planetOfOrigin}";
                if (caughtFish.fish != null) displayName = caughtFish.fish.fishName;
            }
            // Get total value by adding together the value of each fish
            float totalValue = 0f;
            foreach (CaughtFish caughtFish in fishTypes)
            {
                if (caughtFish.fish.name == fish.Key)
                {
                    totalValue += GameManager.CalculateFishValue(caughtFish);
                }
            }
            CreateListItem(displayName, itemIcon, subtext: $"Count: {fish.Value}", subtext2: $"Total Value: {totalValue:F2}", description: itemDescription, mechanicalDescription: itemMechanicalDescription);
        }
        SetupNavigation();
    }

    private void SetupNavigation()
    {
        if (Gamepad.current == null) return;

        if (listItems.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(listItems[0].gameObject);
            OnListItemSelected(0);
        }
    }

    private ListItem CreateListItem(string itemName, Sprite itemIcon, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
    {
        GameObject newItem = Instantiate(listItemPrefab, listContentArea);
        ListItem listItemComponent = newItem.GetComponent<ListItem>();

        // Set the list item data
        if (listItemComponent != null)
        {
            listItemComponent.Init(this, itemName, itemIcon, subtext, subtext2, description, mechanicalDescription, index);
            listItems.Add(listItemComponent);
        }
        else
        {
            Debug.LogError("List item prefab is missing a ListItem component!");
        }
        return listItemComponent;
    }
}
