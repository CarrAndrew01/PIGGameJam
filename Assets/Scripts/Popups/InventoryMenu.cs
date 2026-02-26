using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the inventory menu.
/// </summary>
public class InventoryMenu : MonoBehaviour
{
    // Variables
    [Header("Settings")]
    public Color groupSortOnColor = Color.green; // Color for the sort by group button when active
    public Color groupSortOffColor = Color.white; // Color for the sort by group button

    public static bool sortByGroup = false; // Whether sorting should be done in groups of the same type, or just by order caught

    [Header("Components")]
    public Button sortByGroupButton; // Reference to the button used for sorting
    public TextMeshProUGUI capacityText; // Reference to the text displaying inventory capacity

    private Menu menuComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuComponent = GameManager.MenuPopup.childCanvas.GetComponent<Menu>();
        PopulateList();
        ButtonColorUpdate();
        UpdateCapacityText();
    }

    // Methods
    public void ToggleSortByGroup()
    {
        sortByGroup = !sortByGroup;

        // Repopulate the list to reflect the new sorting method
        PopulateList();
        ButtonColorUpdate();
    }

    private void PopulateList()
    {
        if (menuComponent != null)
        {
            if (!sortByGroup)
                menuComponent.PopulateListWithFish(GameManager.Instance.playerInventory.caughtFish);
            else
                menuComponent.PopulateListWithFishCount(GameManager.Instance.playerInventory.GetFishCountsByType(), GameManager.Instance.playerInventory.caughtFish);
        }
    }
    private void ButtonColorUpdate()
    {
        if (sortByGroup)
            sortByGroupButton.image.color = groupSortOnColor;
        else
            sortByGroupButton.image.color = groupSortOffColor;
    }
    private void UpdateCapacityText()
    {
        float capacity = GameManager.GetPlayerStat(StatType.fishStorage);
        capacityText.text = $"{GameManager.Instance.playerInventory.caughtFish.Count}/{capacity}";
    }
}