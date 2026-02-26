using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;


/// <summary>
/// Handles the shop menu.
/// </summary>
public class ShopMenu : MonoBehaviour
{


    public TextMeshProUGUI descriptionField; // Reference to the TextMeshProUGUI for the description field
    public TextMeshProUGUI mechanicalDescriptionField; // Reference to the TextMeshProUGUI for the mechanical description field

    public Sprite open;
    public Sprite closed;

    public GameObject leftButton;
    public GameObject rightButton;

     public RectTransform listContentArea; // Reference to the RectTransform for the list
    public GameObject listFishPrefab; // Prefab for the list items in the menu

    //ATTENTION: Because we want to remove a specific member of the players inventory, and we cant do direct comparison between CaughtFish,
    //this instead keeps track of the items in the list in the shop, and will adjust their int position value when 1 before them is removed
    //which keeps everything working
    public List<ShopListItem> shopListItems;

    public void BuySwitch()
    {
        leftButton.GetComponent<Image>().sprite = open;
        rightButton.GetComponent<Image>().sprite = closed;

    }

    public void SellSwitch()
    {
        leftButton.GetComponent<Image>().sprite = closed;
        rightButton.GetComponent<Image>().sprite = open;
    }


    public void PopulateListWithFish(List<CaughtFish> fishTypes)
    {
        // Clear existing list items
        foreach (Transform child in listContentArea)
        {
            Destroy(child.gameObject);
        }
 
        // Instantiate new list items based on the provided list of caught fish


        // Instantiate new list items based on the provided list of caught fish
        for (int i = 0; i < GameManager.Instance.playerInventory.caughtFish.Count;i++)
        {
            CaughtFish caughtFish = GameManager.Instance.playerInventory.caughtFish[i];
            //CreateListItem(caughtFish.fish.name, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", subtext2: $"Value: {(caughtFish.weight * 10):F2}", description: caughtFish.fish.description);
        
            GameObject newItem = Instantiate(listFishPrefab, listContentArea);
            ShopListItem listItemComponent = newItem.GetComponent<ShopListItem>();
            

            listItemComponent.Init(this, caughtFish.fish.name, i, caughtFish.fish.sprite, subtext: $"Weight: {caughtFish.weight:F2}", 
            subtext2: $"Value: {(caughtFish.weight * caughtFish.fish.value):F2}", description: caughtFish.fish.description);

            shopListItems.Add(listItemComponent);

        }
    }

    public void AdjustMenuIndexes(int deletedIndex)
    {

        for(int i = deletedIndex + 1; i < shopListItems.Count; i++)
        {
            shopListItems[i].AdjustDown(1);
        }
    }
 
 
    public void SwitchToBuy()
    {
        
        
    }

    public void SwitchToSell()
    {
        

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 
            // TODO: Do stuff here
        PopulateListWithFish(GameManager.Instance.playerInventory.caughtFish);
     }
}
