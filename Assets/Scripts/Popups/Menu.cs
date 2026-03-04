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

    [Header("Input Actions")]
    public InputActionReference navigateDescriptionAction; // Vector2 for scrolling the description field

    [Header("Events")]
    public UnityEvent<int> onItemSelected; // Event that gets triggered when an item is selected, passing the index of the selected item
    public UnityEvent<ListItem> onItemSubmitted; // Event triggered when an item is confirmed with Submit (controller A / keyboard Enter)

    // Components
    [Header("Components")]
    public RectTransform listContentArea; // Reference to the RectTransform for the list
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field
    public TextMeshProUGUI moneyField;

    private ScrollRect descriptionScrollRect;
    private ScrollRect listScrollRect;
    private ContentSizeFitter listContentSizeFitter;
    private GameObject lastKnownSelected;
    protected bool suppressAutoScroll = false;

    [Header("Prefabs")]
    public GameObject listItemPrefab; // Prefab for the list items in the menu

    // Call at the start of every list rebuild to prevent the ContentSizeFitter
    // from shrinking the content rect (and snapping scroll position) while items are being destroyed.
    protected void BeginListRebuild()
    {
        suppressAutoScroll = true;
        if (listContentSizeFitter != null)
            listContentSizeFitter.enabled = false;
    }

    void Awake()
    {
        if (descriptionField != null)
        {
            descriptionScrollRect = descriptionField.GetComponentInParent<ScrollRect>();
            descriptionField.text = "";
        }
        if (listContentArea != null)
        {
            listScrollRect = listContentArea.GetComponentInParent<ScrollRect>();
            listContentSizeFitter = listContentArea.GetComponent<ContentSizeFitter>();
        }
        if (mechanicalDescriptionField != null)
        {
            mechanicalDescriptionField.text = "Click on an item to see its description.";
        }

        UpdateMoneyDisplay();
    }

    public virtual void Update()
    {
        HandleDescriptionNavigation();
        HandleListAutoScroll();
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

    /// <summary>
    /// Called when the player presses Submit on a list item. Override in subclasses or
    /// subscribe to onItemSubmitted to define the confirm action for this menu.
    /// </summary>
    public virtual void OnListItemSubmitted(ListItem item)
    {
        onItemSubmitted?.Invoke(item);
    }

    public void PopulateList(string[] items)
    {
        BeginListRebuild();
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
        BeginListRebuild();
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
        BeginListRebuild();
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
        BeginListRebuild();
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
        BeginListRebuild();
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

    protected void SetupNavigation()
    {
        // List is fully rebuilt — re-enable the ContentSizeFitter so it can measure the new items,
        // then reset scroll to top and allow auto-scroll again.
        if (listContentSizeFitter != null)
            listContentSizeFitter.enabled = true;
        Canvas.ForceUpdateCanvases();
        lastKnownSelected = null;
        if (listScrollRect != null)
            listScrollRect.content.anchoredPosition = Vector2.zero;
        suppressAutoScroll = false;

        if (Gamepad.current == null) return;

        if (listItems.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(listItems[0].gameObject);
            OnListItemSelected(0);
        }
    }

    protected ListItem CreateListItem(string itemName, Sprite itemIcon, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
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

    // Auto-scroll code providing by CoPilot
    private void HandleListAutoScroll()
    {
        if (suppressAutoScroll) return;
        if (listScrollRect == null || EventSystem.current == null) return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || selected == lastKnownSelected) return;
        lastKnownSelected = selected;

        // Only scroll for items that actually live inside this menu's list
        if (!selected.transform.IsChildOf(listContentArea)) return;

        EnsureItemVisible(selected.GetComponent<RectTransform>());
    }

    private void EnsureItemVisible(RectTransform item)
    {
        if (item == null || listScrollRect == null) return;

        Canvas.ForceUpdateCanvases();

        RectTransform viewport = listScrollRect.viewport != null
            ? listScrollRect.viewport
            : listScrollRect.GetComponent<RectTransform>();

        float viewportHeight = viewport.rect.height;
        float contentHeight  = listScrollRect.content.rect.height;

        if (contentHeight <= viewportHeight) return;

        // Get item pivot position in content-local space.
        // Y is negative going downward (content flows top→bottom).
        Vector2 localPos = listScrollRect.content.InverseTransformPoint(item.position);

        // Convert to "distance from content top" so values grow going down.
        float itemTop    = -localPos.y - item.rect.height * (1f - item.pivot.y);
        float itemBottom = itemTop + item.rect.height;

        // content.anchoredPosition.y represents how far the content has been scrolled up.
        // visibleTop..visibleBottom is the currently visible window in "distance from content top".
        float scrollPos     = listScrollRect.content.anchoredPosition.y;
        float visibleTop    = scrollPos;
        float visibleBottom = scrollPos + viewportHeight;

        if (itemBottom > visibleBottom)
        {
            // Item bottom is below the viewport — scroll down to reveal it.
            listScrollRect.content.anchoredPosition = new Vector2(
                listScrollRect.content.anchoredPosition.x,
                itemBottom - viewportHeight);
        }
        else if (itemTop < visibleTop)
        {
            // Item top is above the viewport — scroll up to reveal it.
            listScrollRect.content.anchoredPosition = new Vector2(
                listScrollRect.content.anchoredPosition.x,
                itemTop);
        }
    }

    private void HandleDescriptionNavigation()
    {
        if (navigateDescriptionAction == null || descriptionScrollRect == null) return;

        Vector2 navigationInput = navigateDescriptionAction.action.ReadValue<Vector2>();

        // Scroll vertically based on the y component of the input
        if (Mathf.Abs(navigationInput.y) > 0.1f)
        {
            float scrollAmount = navigationInput.y * Time.deltaTime;
            descriptionScrollRect.verticalNormalizedPosition += scrollAmount;
            descriptionScrollRect.verticalNormalizedPosition = Mathf.Clamp01(descriptionScrollRect.verticalNormalizedPosition);
        }
    }
}
