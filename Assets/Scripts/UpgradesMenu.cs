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
            // TODO: Do stuff here
            menuComponent.PopulateList(new string[] { "Upgrade 1", "Upgrade 2", "Upgrade 3" });
        }
    }
}
