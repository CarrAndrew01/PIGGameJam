using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

/// <summary>
/// Class representing a single item in a list, such as an upgrade or inventory item.
/// </summary>
public class BuyListItem : MonoBehaviour
{
    // Components
    [Header("Data")]

    [Header("Components")]
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI price;

    private ShopMenu parentMenu;
    public Bait baitRef; //the fish that this list item is referencing


    public void Init(Bait fish, ShopMenu parent, string name, int price)
    {
        baitRef = fish;
        parentMenu = parent;
        nameField.text = name;
        this.price.text = $"Price: {price}";
    }

    public void OnItemClicked()
    {
        if (parentMenu != null)
        {
            parentMenu.currentlySelectedBait = baitRef;
            parentMenu.nameField.text = baitRef.baitUpgrade.name;
            parentMenu.priceFieldBuy.text = $"Price: {baitRef.cost}";


            //  parentMenu.descriptionField.text = baitRef.;
            parentMenu.mechanicalDescriptionField.text = baitRef.baitUpgrade.GetMechanicalDescription();
        }
    }
}
