using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BaitMenu : MonoBehaviour
{
    // State
    private bool isBaitTheSelected = false;

    // Variables
    [Header("Settings")]
    public Color selectedBaitColor;
    public Color unselectedBaitColor;
    public string selectBaitText = "Select";
    public string deselectBaitText = "Deselect";

    [Header("Components")]
    public Menu menuComponent;
    public Button selectButton;

    private Image selectButtonImage;
    private TextMeshProUGUI selectButtonText;
    private ListItem selectedListItem;

    void Awake()
    {
        selectButtonImage = selectButton.GetComponent<Image>();
        selectButtonText = selectButton.GetComponentInChildren<TextMeshProUGUI>();

        // Default to the currently selected bait in the inventory when the menu opens
        menuComponent.selectedIndex = GameManager.Instance.playerInventory.baits.FindIndex(b => b.baitUpgrade == GameManager.Instance.playerInventory.currentBaitUpgrade);
        CheckButtonState();
    }


    void Start()
    {
        if (menuComponent != null)
        {
            menuComponent.PopulateListWithBaits(GameManager.Instance.playerInventory.baits);

            // Set the description and mechanical forceably to the first item if it exists
            if (GameManager.Instance.playerInventory.baits.Count > 0 && menuComponent.selectedIndex != -1)
            {
                menuComponent.listItems[menuComponent.selectedIndex].SetDescriptionFields();
                menuComponent.listItems[menuComponent.selectedIndex].SetSelected(true);
            }
        }

        CheckStampState();
    }

    public void OnListItemSelected()
    {
        CheckButtonState();
    }

    public void SelectBait()
    {
        int baitIndex = menuComponent.selectedIndex;
        if (baitIndex != -1 && baitIndex < GameManager.Instance.playerInventory.baits.Count)
        {
            Bait selectedBait = GameManager.Instance.playerInventory.baits[baitIndex];
            // Toggle selection: if this bait is already selected, deselect it; otherwise select it
            if (selectedBait.baitUpgrade == GameManager.Instance.playerInventory.currentBaitUpgrade)
            {
                GameManager.DeselectBaitUpgrade();

                CheckStampState();
            }
            else
            {
                GameManager.SelectBaitUpgrade(selectedBait.baitUpgrade);

                CheckStampState();
            }
            CheckButtonState();
        }
        else
        {
            Debug.LogWarning("Invalid bait index selected.");
        }
    }

    private void CheckStampState()
    {
        if (menuComponent.selectedIndex != -1 && menuComponent.selectedIndex < GameManager.Instance.playerInventory.baits.Count)
        {
            Bait selectedBait = GameManager.Instance.playerInventory.baits[menuComponent.selectedIndex];
            bool isCurrentlySelected = selectedBait.baitUpgrade == GameManager.Instance.playerInventory.currentBaitUpgrade;

            // Update stamp state based on whether the bait is currently selected
            if (isCurrentlySelected)
            {
                if (selectedListItem != null)
                {
                    selectedListItem.Unstamp();
                }

                menuComponent.listItems[menuComponent.selectedIndex].Stamp();
                selectedListItem = menuComponent.listItems[menuComponent.selectedIndex];
            }
            else if (!isCurrentlySelected)
            {
                menuComponent.listItems[menuComponent.selectedIndex].Unstamp();
                selectedListItem = null;
            }
        }
    }

    private void CheckButtonState()
    {
        if (menuComponent.selectedIndex != -1 && menuComponent.selectedIndex < GameManager.Instance.playerInventory.baits.Count)
        {
            Bait selectedBait = GameManager.Instance.playerInventory.baits[menuComponent.selectedIndex];
            isBaitTheSelected = selectedBait.baitUpgrade == GameManager.Instance.playerInventory.currentBaitUpgrade;

            // Always allow pressing the button; pressing again will deselect
            selectButton.interactable = true;
            selectButtonImage.color = isBaitTheSelected ? selectedBaitColor : unselectedBaitColor;
            selectButtonText.text = isBaitTheSelected ? deselectBaitText : selectBaitText;
        }
        else
        {
            selectButton.interactable = false; // Disable if no valid selection
            selectButtonImage.color = unselectedBaitColor; // Set to unselected color when no selection is valid
            selectButtonText.text = selectBaitText; // Reset text to default when no selection is valid
        }
    }
}
