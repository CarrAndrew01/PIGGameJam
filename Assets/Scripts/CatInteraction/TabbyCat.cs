using UnityEngine;

public class TabbyCat : CatInteract
{
    private MenuManager menus;

    public override void Start()
    {
        base.Start();
        menus = GameManager.MenuPopup.GetComponent<MenuManager>();
    }

    public override void InteractWithCat()
    {
        base.InteractWithCat();

        if (menus != null)
            menus.TriggerShopMenu();
        else
            Debug.LogWarning("Menus component not found on MenuPopup - cannot trigger shop menu.");
    }
}