using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

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
    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field

    [Header("Prefabs")]
    public GameObject listItemPrefab; // Prefab for the list items in the menu

    void Start()
    {
        descriptionField.text = "";
        mechanicalDescriptionField.text = "Click on an item to see its description.";
    }

    // Methods
    public void OnListItemSelected(int index)
    {
        selectedIndex = index;
        onItemSelected?.Invoke(index);

        // Update selection highlight for all list items
        foreach (ListItem item in listItems)
        {
            item.UpdateSelectionHighlight();
        }
    }

    public void PopulateList(string[] items)
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided array of strings
        foreach (string item in items)
        {
            CreateListItem(item, null);
        }
    }


    public void PopulateListWithUpgrades(List<Upgrade> upgrades)
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided list of upgrades
        for (int i = 0; i < upgrades.Count; i++)
        {
            Upgrade upgrade = upgrades[i];
            CreateListItem(upgrade.name, upgrade.icon, description: upgrade.description, mechanicalDescription: upgrade.GetMechanicalDescription(), index: i);
        }
    }
    public void PopulateListWithBaits(List<Bait> baits)
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided list of baits
        for (int i = 0; i < baits.Count; i++)
        {
            Bait bait = baits[i];
            CreateListItem(bait.baitUpgrade.name, bait.baitUpgrade.icon, subtext: $"Uses: {bait.numberOfUses}", description: bait.baitUpgrade.description, mechanicalDescription: bait.mechanicalDescription, index: i);
        }
    }
    public void PopulateListWithFish(List<CaughtFish> fishTypes)
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided list of caught fish
        for (int i = 0; i < fishTypes.Count; i++)
        {
            CaughtFish caughtFish = fishTypes[i];
            CreateListItem(caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {(caughtFish.weight * 10):F2}", description: caughtFish.fish.description, index: i);
        }
    }

    public void PopulateListWithFishCount(Dictionary<string, int> fishCount, List<CaughtFish> fishTypes = null)
    {
        // Clear existing list items
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
            if (fishTypes != null)
            {
                CaughtFish caughtFish = fishTypes.Find(f => f.fish.name == fish.Key);
                itemIcon = caughtFish.fish.sprite;
                itemDescription = caughtFish.fish.description;
            }
            // Get total value by adding together the value of each fish
            float totalValue = 0f;
            foreach (CaughtFish caughtFish in fishTypes)
            {
                if (caughtFish.fish.name == fish.Key)
                {
                    totalValue += caughtFish.weight * 10;
                }
            }
            CreateListItem(fish.Key, itemIcon, subtext: $"Count: {fish.Value:F2}", subtext2: $"Total Value: {totalValue:F2}", description: itemDescription);
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
