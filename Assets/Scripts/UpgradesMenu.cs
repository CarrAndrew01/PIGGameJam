using UnityEngine;

/// <summary>
/// Handles the upgrades menu.
/// </summary>
public class UpgradesMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Menu menuComponent = GameManager.MenuPopup.childCanvas.GetComponent<Menu>();
        if (menuComponent != null)
        {
            menuComponent.PopulateListWithUpgrades(GameManager.Instance.playerStats.upgrades);
        }
    }
}
