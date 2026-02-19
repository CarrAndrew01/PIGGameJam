using UnityEngine;

/// <summary>
/// Handles the inventory menu.
/// </summary>
public class InventoryMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Menu menuComponent = GameManager.MenuPopup.childCanvas.GetComponent<Menu>();
        if (menuComponent != null)
        {
            // TODO: Do stuff here
            menuComponent.PopulateListWithFishCount(GameManager.Instance.playerInventory.GetFishCountsByType(), GameManager.Instance.playerInventory.caughtFish);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
