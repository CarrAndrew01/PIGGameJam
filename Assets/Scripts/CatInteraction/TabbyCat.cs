using UnityEngine;

public class TabbyCat : CatInteract
{
    private Menus menus;

    public override void Start()
    {
        base.Start();
        menus = GameManager.MenuPopup.GetComponent<Menus>();
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