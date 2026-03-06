using System;
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
    /// <summary>
    /// Struct to hold all relevant data for a list item in the menu. Helps clean up the methods.
    /// </summary>
    protected struct ListItemData
    {
        public string name;
        public Sprite icon;
        public string subtext;
        public string subtext2;
        public string description;
        public string mechanicalDescription;
        public int index;

        public ListItemData(string name, Sprite icon = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
        {
            this.name = name;
            this.icon = icon;
            this.subtext = subtext;
            this.subtext2 = subtext2;
            this.description = description;
            this.mechanicalDescription = mechanicalDescription;
            this.index = index;
        }
    }

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
    private StableScrollRect listScrollRect;
    private Canvas listCanvas;
    private GameObject lastKnownSelected;
    protected bool suppressAutoScroll = false;

    [Header("Prefabs")]
    public GameObject listItemPrefab; // Prefab for the list items in the menu

    protected void BeginListRebuild()
    {
        suppressAutoScroll = true;
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
            listScrollRect = listContentArea.GetComponentInParent<StableScrollRect>();
            listCanvas = listContentArea.GetComponentInParent<Canvas>();
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

    public void UpdateMoneyDisplay()
    {
        if (moneyField != null) moneyField.text = $"${GameManager.Money:F2}";
    }

    // MARK: POPULATE METHODS
    public void PopulateListWithStrings(string[] items)
    {
        PopulateList<string>(items, (item, i) => new ListItemData(item));
    }

    public void PopulateListWithUpgrades(List<Upgrade> upgrades)
    {
        PopulateList(upgrades, (upgrade, i) =>
        new ListItemData(
            upgrade.upgradeName, upgrade.icon,
            description: upgrade.description,
            mechanicalDescription: upgrade.GetMechanicalDescription(), index: i)
            );
    }
    public void PopulateListWithBaits(List<Bait> baits)
    {
        PopulateList(baits, (bait, i) =>
        new ListItemData(
            bait.baitUpgrade.upgradeName, bait.baitUpgrade.icon,
            subtext: $"Uses: {bait.numberOfUses}",
            description: bait.baitUpgrade.description,
            mechanicalDescription: bait.baitUpgrade.GetMechanicalDescription(), index: i)
            );
    }
    public void PopulateListWithFish(List<CaughtFish> fishTypes)
    {
        PopulateList(fishTypes, (caughtFish, i) =>
        new ListItemData(
            caughtFish.fish.fishName, caughtFish.fish.sprite,
            subtext: $"Weight: {caughtFish.weight:F2}",
            subtext2: $"Value: {GameManager.CalculateFishValue(caughtFish):F2}",
            description: caughtFish.fish.description,
            mechanicalDescription: $"Planet of origin: {caughtFish.planetOfOrigin}", index: i)
            );
    }

    public void PopulateListWithFishCount(Dictionary<string, int> fishCount, List<CaughtFish> fishTypes = null)
    {
        var entries = new List<KeyValuePair<string, int>>(fishCount);
        PopulateList(entries, (fish, i) =>
        {
            Sprite itemIcon = null;
            string itemDescription = "";
            string itemMechanicalDescription = "";
            string displayName = fish.Key;
            if (fishTypes != null)
            {
                CaughtFish found = fishTypes.Find(f => f.fish.name == fish.Key);
                itemIcon = found.fish.sprite;
                itemDescription = found.fish.description;
                itemMechanicalDescription = $"Planet of origin: {found.planetOfOrigin}";
                if (found.fish != null) displayName = found.fish.fishName;
            }
            float totalValue = 0f;
            if (fishTypes != null)
                foreach (CaughtFish cf in fishTypes)
                    if (cf.fish.name == fish.Key)
                        totalValue += GameManager.CalculateFishValue(cf);
            return new ListItemData(displayName, itemIcon,
                subtext: $"Count: {fish.Value}",
                subtext2: $"Total Value: {totalValue:F2}",
                description: itemDescription,
                mechanicalDescription: itemMechanicalDescription);
        });
    }

    protected void SetupNavigation(int preserveIndex = -1)
    {
        // if (listCanvas != null)
        // LayoutRebuilder.ForceRebuildLayoutImmediate(listCanvas.GetComponent<RectTransform>());
        lastKnownSelected = null;
        // For making the slider remain in the same position when rebuilding
        listScrollRect.SuppressNextLayoutJump();
        suppressAutoScroll = false;

        if (Gamepad.current == null) return;

        // If on controller, select the next logical item.
        if (listItems.Count > 0)
        {
            int newIndex = Mathf.Clamp(preserveIndex < 0 ? 0 : preserveIndex, 0, listItems.Count - 1);
            // Clear first so the EventSystem always fires OnSelect, even when the same recycled GO is re-selected.
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(listItems[newIndex].gameObject);
            OnListItemSelected(newIndex);
        }
    }

    // Generic method for populating the list with all our data types.
    protected void PopulateList<T>(IReadOnlyList<T> items, Func<T, int, ListItemData> mapper)
    {
        BeginListRebuild();
        int preserveIndex = selectedIndex; // Capture before anything during rebuild can change it
        List<ListItem> oldItems = new List<ListItem>(listItems);
        listItems.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            ListItemData d = mapper(items[i], i);
            RecycleOrCreateListItem(oldItems, i, d.name, d.icon, d.subtext, d.subtext2, d.description, d.mechanicalDescription, d.index);
        }

        // Deactivate before destroying so the layout immediately reflects the new item count
        for (int i = items.Count; i < oldItems.Count; i++)
        {
            oldItems[i].gameObject.SetActive(false);
            Destroy(oldItems[i].gameObject);
        }

        SetupNavigation(preserveIndex);
    }

    // Overload for custom prefabs where the caller handles the item initialization
    protected void PopulateList<T>(IReadOnlyList<T> items, GameObject prefabOverride, Action<ListItem, T, int> initItem)
    {
        BeginListRebuild();
        int preserveIndex = selectedIndex;
        List<ListItem> oldItems = new List<ListItem>(listItems);
        listItems.Clear();

        // Determine the expected ListItem type from the prefab
        System.Type expectedType = prefabOverride.GetComponent<ListItem>()?.GetType();

        for (int i = 0; i < items.Count; i++)
        {
            ListItem listItem;
            if (i < oldItems.Count && (expectedType == null || oldItems[i].GetType() == expectedType))
            {
                listItem = oldItems[i];
            }
            else
            {
                // Wrong type or no old item — destroy the old one (if any) and instantiate fresh
                if (i < oldItems.Count)
                {
                    oldItems[i].gameObject.SetActive(false);
                    Destroy(oldItems[i].gameObject);
                }
                GameObject obj = Instantiate(prefabOverride, listContentArea);
                listItem = obj.GetComponent<ListItem>();
                if (listItem == null)
                {
                    Debug.LogError("Prefab is missing a ListItem component!");
                    Destroy(obj);
                    continue;
                }
            }
            listItems.Add(listItem);
            initItem(listItem, items[i], i);
            listItem.listIndex = i;
        }

        // Destroy surplus items beyond the new count
        for (int i = items.Count; i < oldItems.Count; i++)
        {
            oldItems[i].gameObject.SetActive(false);
            Destroy(oldItems[i].gameObject);
        }

        SetupNavigation(preserveIndex);
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

    // Reuses an existing ListItem at reuseIndex if available, otherwise instantiates a new one.
    protected ListItem RecycleOrCreateListItem(List<ListItem> oldItems, int reuseIndex, string itemName, Sprite itemIcon, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
    {
        ListItem listItemComponent;
        if (reuseIndex < oldItems.Count)
        {
            listItemComponent = oldItems[reuseIndex];
        }
        else
        {
            GameObject newItem = Instantiate(listItemPrefab, listContentArea);
            listItemComponent = newItem.GetComponent<ListItem>();
            if (listItemComponent == null)
            {
                Debug.LogError("List item prefab is missing a ListItem component!");
                return null;
            }
        }
        listItemComponent.Init(this, itemName, itemIcon, subtext, subtext2, description, mechanicalDescription, index);
        listItems.Add(listItemComponent);
        return listItemComponent;
    }

    // Auto-scroll code providing by CoPilot
    private void HandleListAutoScroll()
    {
        // if (suppressAutoScroll) return;
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

        // if (listCanvas != null)
        //     LayoutRebuilder.ForceRebuildLayoutImmediate(listCanvas.GetComponent<RectTransform>());

        RectTransform viewport = listScrollRect.viewport != null
            ? listScrollRect.viewport
            : listScrollRect.GetComponent<RectTransform>();

        float viewportHeight = viewport.rect.height;
        float contentHeight = listScrollRect.content.rect.height;

        if (contentHeight <= viewportHeight) return;

        // Get item pivot position
        Vector2 localPos = listScrollRect.content.InverseTransformPoint(item.position);

        // Convert to distance from top so values grow going down
        float itemTop = -localPos.y - item.rect.height * (1f - item.pivot.y);
        float itemBottom = itemTop + item.rect.height;

        // Get current scroll position in distance from top
        float scrollPos = listScrollRect.content.anchoredPosition.y;
        float visibleTop = scrollPos;
        float visibleBottom = scrollPos + viewportHeight;

        if (itemBottom > visibleBottom)
        {
            // Item bottom is below the viewport
            listScrollRect.content.anchoredPosition = new Vector2(
                listScrollRect.content.anchoredPosition.x,
                itemBottom - viewportHeight);
        }
        else if (itemTop < visibleTop)
        {
            // Item top is above the viewport
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
