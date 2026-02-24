using UnityEngine;

public class TabbyCat : CatInteract
{
   
    public override void InteractWithCat()
    {
        //base.InteractWithCat();
                Debug.Log("123");

        // For now, just give the player 10 money when they interact with the tabby cat, but later this will be replaced with a dialogue sequence where the player can choose to pet the cat or not, and if they pet it will give them a small amount of money and increase their happiness, and if they don't pet it nothing will happen
        GameManager.AdjustMoney(10);
    }
}