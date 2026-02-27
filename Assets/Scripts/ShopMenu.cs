using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Handles the shop menu.
/// </summary>
public class ShopMenu : MonoBehaviour
{

    public TextMeshProUGUI nameField;
    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field

    public TextMeshProUGUI priceFieldBuy;
    public GameObject buyButton;


    public Sprite open;
    public Sprite closed;

    public GameObject leftButton;
    public GameObject rightButton;

     public RectTransform listContentArea; // Reference to the RectTransform for the list
    public GameObject listFishPrefab; // Prefab for the list items in the menu

    public GameObject listBaitPrefab; // Prefab for the list items in the menu

    public Bait currentlySelectedBait;

    public List<Bait> allBait = new();
 

    public void BuySwitch()
    {
        leftButton.GetComponent<Image>().sprite = open;
        rightButton.GetComponent<Image>().sprite = closed;

        mechanicalDescriptionField.gameObject.SetActive(true);
        priceFieldBuy.gameObject.SetActive(true);
        buyButton.SetActive(true);

        PopulateListWithBait();
    }

    public void SellSwitch()
    {
        leftButton.GetComponent<Image>().sprite = closed;
        rightButton.GetComponent<Image>().sprite = open;

        mechanicalDescriptionField.gameObject.SetActive(false);
        priceFieldBuy.gameObject.SetActive(false);
        buyButton.SetActive(false);

        PopulateListWithFish(GameManager.Instance.playerInventory.caughtFish);
    }


    public void BuyButton()
    {
        int price = currentlySelectedBait.cost;

        if(GameManager.Instance.money >= price){
            GameManager.Instance.money -= price;
            GameManager.Instance.playerInventory.AddBait(currentlySelectedBait.baitUpgrade, 1, currentlySelectedBait.baitUpgrade.description);
        }
    }


    public void PopulateListWithBait()
    {
        //make the bait list

        //so loop
        
        
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }
        
        // Instantiate new list items based on the provided list of caught fish
        for (int i = 0; i < allBait.Count; i++)
        {
            Bait currentBait = allBait[i];
            //CreateListItem(caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {(caughtFish.weight * 10):F2}", description: caughtFish.fish.description);
        
            GameObject newItem = Instantiate(listBaitPrefab, listContentArea);
            BuyListItem listItemComponent = newItem.GetComponent<BuyListItem>();
            

            listItemComponent.Init(currentBait, this, currentBait.baitUpgrade.name, currentBait.cost);

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
        for (int i = 0; i < GameManager.Instance.playerInventory.caughtFish.Count;i++)
        {
            CaughtFish caughtFish = GameManager.Instance.playerInventory.caughtFish[i];
            //CreateListItem(caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {(caughtFish.weight * 10):F2}", description: caughtFish.fish.description);
        
            GameObject newItem = Instantiate(listFishPrefab, listContentArea);
            SellListItem listItemComponent = newItem.GetComponent<SellListItem>();
            

            listItemComponent.Init(caughtFish, this, caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", 
            subtext2: $"Value: {(caughtFish.weight * caughtFish.fish.value):F2}", description: caughtFish.fish.description);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Do stuff here
        //PopulateListWithFish(GameManager.Instance.playerInventory.caughtFish);
     }
}
