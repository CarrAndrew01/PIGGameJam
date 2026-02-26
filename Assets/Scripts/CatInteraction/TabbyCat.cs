using UnityEngine;

public class TabbyCat : CatInteract
{
    public GameObject shopMenu;

    public override void InteractWithCat()
    {
        //base.InteractWithCat();
        
    
        // For now, just give the player 10 money when they interact with the tabby cat, but later this will be replaced with a dialogue sequence where the player can choose to pet the cat or not, and if they pet it will give them a small amount of money and increase their happiness, and if they don't pet it nothing will happen
        TempMakeMenu();
    }    
    public void TempMakeMenu()
    {
        shopMenu.SetActive(true);
        //shopMenu.GetComponentInChildren<ShopMenu>().PopulateShopList();
    }

}