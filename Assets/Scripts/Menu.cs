using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Generic menu class used for various menus with a list of items.
/// </summary>
public class Menu : MonoBehaviour
{
    // Components
    [Header("Components")]
    public RectTransform listContentArea; // Reference to the RectTransform for the list
    public GameObject listItemPrefab; // Prefab for the list items in the menu
    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field

    void Start()
    {
        descriptionField.text = "";
        mechanicalDescriptionField.text = "Click on an item to see its description.";
    }

    // Methods
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
        foreach (Upgrade upgrade in upgrades)
        {
            CreateListItem(upgrade.name, upgrade.icon, description: upgrade.description, mechanicalDescription: upgrade.GetMechanicalDescription());
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
        foreach (CaughtFish caughtFish in fishTypes)
        {
            CreateListItem(caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {(caughtFish.weight * 10):F2}", description: caughtFish.fish.description);
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

    private ListItem CreateListItem(string itemName, Sprite itemIcon, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "")
    {
        GameObject newItem = Instantiate(listItemPrefab, listContentArea);
        ListItem listItemComponent = newItem.GetComponent<ListItem>();

        // Set the list item data
        if (listItemComponent != null)
        {
            listItemComponent.Init(this, itemName, itemIcon, subtext, subtext2, description, mechanicalDescription);
        }
        else
        {
            Debug.LogError("List item prefab is missing a ListItem component!");
        }
        return listItemComponent;
    }
}
