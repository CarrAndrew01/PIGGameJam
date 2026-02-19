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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            GameObject newItem = Instantiate(listItemPrefab, listContentArea);
            TextMeshProUGUI itemText = newItem.GetComponentInChildren<TextMeshProUGUI>();
            if (itemText != null)
            {
                itemText.text = item;
            }
            else
            {
                Debug.LogError("List item prefab is missing a TextMeshPro component!");
            }
        }
    }
    public void PopulateList(Dictionary<string, int> items, List<CaughtFish> fishTypes = null)
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new list items based on the provided dictionary of strings and ints
        foreach (KeyValuePair<string, int> item in items)
        {
            GameObject newItem = Instantiate(listItemPrefab, listContentArea);
            TextMeshProUGUI itemText = newItem.GetComponentInChildren<TextMeshProUGUI>();
            Image itemImage = null;
            foreach (Image img in newItem.GetComponentsInChildren<Image>())
            {
                if (img.gameObject != newItem) // Ensure we don't accidentally grab the background image of the list item
                {
                    itemImage = img;
                    break;
                }
            }

            // Check key against fish types in player inventory to extract sprite
            if (fishTypes != null && fishTypes.Count > 0)
            {
                CaughtFish caughtFish = fishTypes.Find(f => f.fish.name == item.Key);
                if (itemImage != null)
                {
                    itemImage.sprite = caughtFish.fish.sprite;
                }
            }

            if (itemText != null)
            {
                itemText.text = $"{item.Key}: {item.Value}";
            }
            else
            {
                Debug.LogError("List item prefab is missing a TextMeshPro component!");
            }
        }
    }
}
