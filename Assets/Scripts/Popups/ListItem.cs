using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class ListItem : MonoBehaviour
{
    // Components
    [Header("Data")]
    public string description;
    public string mechanicalDescription;
    public int listIndex; // Index of this item in the list, set by the Menu when creating the item

    [Header("Components")]
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI subtextField;
    public TextMeshProUGUI subtextField2;
    public Image icon;
    public Image selectHightlight;

    private Menu parentMenu;

    public void Init(Menu parent, string name, Sprite iconSprite = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
    {
        parentMenu = parent;
        nameField.text = name;
        subtextField.text = subtext;
        subtextField2.text = subtext2;
        listIndex = index;
        icon.sprite = iconSprite;
        this.description = description;
        this.mechanicalDescription = mechanicalDescription;

        // Set icon visibility based on whether an icon was provided
        icon.transform.parent.gameObject.SetActive(iconSprite != null);

        // Set the subtexts visibility based on whether they were provided
        subtextField.gameObject.SetActive(!string.IsNullOrEmpty(subtext));
        subtextField2.gameObject.SetActive(!string.IsNullOrEmpty(subtext2));

        // Enable the select object
        UpdateSelectionHighlight();
    }

    public void OnItemClicked()
    {
        if (parentMenu != null)
        {
            parentMenu.descriptionField.text = description;
            parentMenu.mechanicalDescriptionField.text = mechanicalDescription;
            if (listIndex != -1)
                parentMenu.OnListItemSelected(listIndex);
        }
    }

    public void UpdateSelectionHighlight()
    {
        if (parentMenu != null && parentMenu.selectedIndex == listIndex)
            selectHightlight.gameObject.SetActive(true);
        else
            selectHightlight.gameObject.SetActive(false);
    }
}
