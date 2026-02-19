using UnityEngine;

/// <summary>
/// Handles the shop menu.
/// </summary>
public class ShopMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Menu menuComponent = GameManager.MenuPopup.childCanvas.GetComponent<Menu>();
        if (menuComponent != null)
        {
            // TODO: Do stuff here
            menuComponent.PopulateList(new string[] { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" });
        }
    }
}
