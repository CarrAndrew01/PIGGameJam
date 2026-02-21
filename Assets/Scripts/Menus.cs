using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Handles opening and closing various menus.
/// </summary>
public class Menus : MonoBehaviour
{
    public enum MenuType
    {
        None,
        MainMenu,
        UpgradeMenu,
        InventoryMenu,
        ShopMenu
    }


    [Header("Input Actions")]
    public InputActionReference menuAction; // expects Button
    public InputActionReference upgradeMenuAction; // expects Button
    public InputActionReference inventoryMenuAction; // expects Button

    [Header("Prefabs")]
    public GameObject menuPrefab; // Prefab for the escape menu popup
    public GameObject upgradeMenuPrefab; // Prefab for the upgrade menu popup
    public GameObject inventoryMenuPrefab; // Prefab for the inventory menu popup
    public GameObject shopMenuPrefab; // Prefab for the shop menu popup

    [Header("Debug")]
    [ShowInInspector,ReadOnly] private GameObject currentMenu; // Reference to the currently open menu, if any
    [ShowInInspector,ReadOnly] private MenuType currentMenuType; // Type of the currently open menu, if any

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.MenuPopup.ReadyForInput)
            return; // Don't allow menu input if the popup is currently animating or problems occur
            
        if (menuAction.action.WasPressedThisFrame())
        {
            if (currentMenu != null)
            {
                // If any menu is open, close it (never swap to main menu)
                GameManager.TriggerPopOut(GameManager.MenuPopup);
                currentMenu = null;
                currentMenuType = MenuType.None;
            }
            else
            {
                // Only open the main menu if no menu is currently open
                OpenMenu(MenuType.MainMenu);
                currentMenuType = MenuType.MainMenu;
            }
        }
        else if (upgradeMenuAction.action.WasPressedThisFrame())
        {
            if (currentMenu != null && currentMenuType == MenuType.UpgradeMenu)
            {
                // If the upgrade menu is already open, close it
                GameManager.TriggerPopOut(GameManager.MenuPopup);
                currentMenu = null;
            }
            else
            {
                // Otherwise, open the upgrade menu
                OpenMenu(MenuType.UpgradeMenu);
                currentMenuType = MenuType.UpgradeMenu;
            }
        }
        else if (inventoryMenuAction.action.WasPressedThisFrame())
        {
            if (currentMenu != null && currentMenuType == MenuType.InventoryMenu)
            {
                // If the inventory menu is already open, close it
                GameManager.TriggerPopOut(GameManager.MenuPopup);
                currentMenu = null;
            }
            else
            {
                // Otherwise, open the inventory menu
                OpenMenu(MenuType.InventoryMenu);
                currentMenuType = MenuType.InventoryMenu;
            }
        }
    }

    public void TriggerShopMenu()
    {
        if (currentMenu != null && currentMenuType == MenuType.ShopMenu)
        {
            // If the shop menu is already open, don't do anything (maybe, I don't know)
        }
        else
        {
            // Otherwise, open the shop menu
            OpenMenu(MenuType.ShopMenu);
            currentMenuType = MenuType.ShopMenu;
        }
    }

    private void OpenMenu(MenuType menuType)
    {
        if (!GameManager.MenuPopup.ReadyForInput)
            return; // Don't open a new menu if the popup is currently animating or problems occur

        // Helper method to open a menu of the given type
        switch (menuType)
        {
            case MenuType.None:
                // Don't open any menu
                break;
            case MenuType.MainMenu:
                if (menuPrefab == null)
                {
                    Debug.LogError("Menu prefab reference is missing in Menus component!");
                    return;
                }
                if (currentMenu != null)
                {
                    // If another menu is already open, swap to the new menu instead
                    StartCoroutine(SwapMenu(menuType));
                    return;
                }
                currentMenu = GameManager.TriggerPopIn(GameManager.MenuPopup, menuPrefab);
                break;
            case MenuType.UpgradeMenu:
                if (upgradeMenuPrefab == null)
                {
                    Debug.LogError("Upgrade menu prefab reference is missing in Menus component!");
                    return;
                }
                if (currentMenu != null)
                {
                    // If another menu is already open, swap to the new menu instead
                    StartCoroutine(SwapMenu(menuType));
                    return;
                }
                currentMenu = GameManager.TriggerPopIn(GameManager.MenuPopup, upgradeMenuPrefab);
                break;
            case MenuType.InventoryMenu:
                if (inventoryMenuPrefab == null)
                {
                    Debug.LogError("Inventory menu prefab reference is missing in Menus component!");
                    return;
                }
                if (currentMenu != null)
                {
                    // If another menu is already open, swap to the new menu instead
                    StartCoroutine(SwapMenu(menuType));
                    return;
                }
                currentMenu = GameManager.TriggerPopIn(GameManager.MenuPopup, inventoryMenuPrefab);
                break;
            case MenuType.ShopMenu:
                if (shopMenuPrefab == null)
                {
                    Debug.LogError("Shop menu prefab reference is missing in Menus component!");
                    return;
                }
                if (currentMenu != null)
                {
                    // If another menu is already open, swap to the new menu instead
                    StartCoroutine(SwapMenu(menuType));
                    return;
                }
                currentMenu = GameManager.TriggerPopIn(GameManager.MenuPopup, shopMenuPrefab);
                break;
            default:
                Debug.LogError($"Unhandled menu type {menuType} in OpenMenu!");
                break;
        }

        if (currentMenu == null)
            Debug.Log($"Failed to trigger {menuType} popup - something may already be open.");
    }

    private IEnumerator SwapMenu(MenuType newMenuType)
    {
        if (!GameManager.MenuPopup.ReadyForInput)
            yield break; // Don't swap menus if the popup is currently animating or problems occur
        Debug.Log($"Swapping from {currentMenuType} to {newMenuType} menu.");
        GameObject menuPrefabToUse = null;
        switch (newMenuType)
        {
            case MenuType.None:
                Debug.LogWarning("Attempted to swap to None menu type, which is not a real menu. This is likely a bug.");
                yield break;
            case MenuType.MainMenu:
                menuPrefabToUse = menuPrefab;
                break;
            case MenuType.UpgradeMenu:
                menuPrefabToUse = upgradeMenuPrefab;
                break;
            case MenuType.InventoryMenu:
                menuPrefabToUse = inventoryMenuPrefab;
                break;
            case MenuType.ShopMenu:
                menuPrefabToUse = shopMenuPrefab;
                break;
            default:
                Debug.LogError($"Unhandled menu type {newMenuType} in SwapMenu!");
                yield break;
        }
        GameObject result = null;
        yield return StartCoroutine(GameManager.TriggerSwap(GameManager.MenuPopup, menuPrefabToUse, go => result = go));
        currentMenu = result;
    }
}