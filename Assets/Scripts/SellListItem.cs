using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class SellListItem : MonoBehaviour
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
    private ShopMenu parentMenu;
    
    public CaughtFish fishRef; //the fish that this list item is referencing
    
    public void Init(CaughtFish fish, ShopMenu parent, string name, Sprite iconSprite = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "")
    {
        fishRef = fish;
        parentMenu = parent;
        nameField.text = name;
        subtextField.text = subtext;
        subtextField2.text = subtext2;
        icon.sprite = iconSprite;
        this.description = description;
        this.mechanicalDescription = mechanicalDescription;

        // Set icon visibility based on whether an icon was provided
        icon.gameObject.SetActive(iconSprite != null);

        // Set the subtexts visibility based on whether they were provided
        subtextField.gameObject.SetActive(!string.IsNullOrEmpty(subtext));
        subtextField2.gameObject.SetActive(!string.IsNullOrEmpty(subtext2));
    }

    public void OnSellClicked()
    {
        //handle the rest of the logic


        GameManager.Instance.playerInventory.RemoveFishByID(fishRef.id);
        GameManager.Instance.money += fishRef.fish.value * fishRef.weight;

        Destroy(gameObject);
    }



    public void OnItemClicked()
    {
        if (parentMenu != null)
        {
            parentMenu.nameField.text = fishRef.fish.name;
            parentMenu.descriptionField.text = description;
           // parentMenu.mechanicalDescriptionField.text = mechanicalDescription;
        }
    }
}
