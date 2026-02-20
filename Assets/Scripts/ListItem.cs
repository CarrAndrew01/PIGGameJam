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

    [Header("Components")]
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI subtextField;
    public TextMeshProUGUI subtextField2;
    public Image icon;

    private Menu parentMenu;
    
    public void Init(Menu parent, string name, Sprite iconSprite = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "")
    {
        parentMenu = parent;
        nameField.text = name;
        subtextField.text = subtext;
        subtextField2.text = subtext2;
        icon.sprite = iconSprite;
        this.description = description;
        this.mechanicalDescription = mechanicalDescription;

        // Set icon visibility based on whether an icon was provided
        icon.gameObject.SetActive(iconSprite != null);
    }

    public void OnItemClicked()
    {
        if (parentMenu != null)
        {
            parentMenu.descriptionField.text = description;
            parentMenu.mechanicalDescriptionField.text = mechanicalDescription;
        }
    }
}
