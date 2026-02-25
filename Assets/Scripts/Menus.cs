using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles opening and closing various menus.
/// </summary>
public class Menus : MonoBehaviour
{
    public static Menus Instance { get; private set; }

    public static bool IsAnyMenuOpen => Instance.currentMenu != null;

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
    public InputActionReference shopMenuAction; // expects Button

    [Header("Prefabs")]
    public GameObject menuPrefab; // Prefab for the escape menu popup
    public GameObject upgradeMenuPrefab; // Prefab for the upgrade menu popup
    public GameObject inventoryMenuPrefab; // Prefab for the inventory menu popup
    public GameObject shopMenuPrefab; // Prefab for the shop menu popup

    [Header("Debug")]
    [ShowInInspector, ReadOnly] private GameObject currentMenu; // Reference to the currently open menu, if any
    [ShowInInspector, ReadOnly] private MenuType currentMenuType; // Type of the currently open menu, if any

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.MenuPopup.ReadyForInput)
            return; // Don't allow menu input if the popup is currently animating or problems occur

        if (menuAction.action.WasPressedThisFrame())
        {
            if (currentMenu != null)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Only open the main menu if no menu is currently open
                OpenMenu(MenuType.MainMenu, priority: true);
                currentMenuType = MenuType.MainMenu;
            }
        }
        else if (upgradeMenuAction.action.WasPressedThisFrame())
        {
            if (currentMenu != null && currentMenuType == MenuType.UpgradeMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the upgrade menu
                OpenMenu(MenuType.UpgradeMenu, priority: true);
                currentMenuType = MenuType.UpgradeMenu;
            }
        }
        else if (inventoryMenuAction.action.WasPressedThisFrame())
        {
            if (currentMenu != null && currentMenuType == MenuType.InventoryMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the inventory menu
                OpenMenu(MenuType.InventoryMenu, priority: true);
                currentMenuType = MenuType.InventoryMenu;
            }
        }
        // else if (shopMenuAction.action.WasPressedThisFrame())
        // {
        //     if (currentMenu != null && currentMenuType == MenuType.ShopMenu)
        //     {
        //         CloseCurrentMenu();
        //     }
        //     else
        //     {
        //         // Otherwise, open the shop menu
        //         OpenMenu(MenuType.ShopMenu, priority: true);
        //         currentMenuType = MenuType.ShopMenu;
        //     }
        // }
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
            OpenMenu(MenuType.ShopMenu, priority: true);
            currentMenuType = MenuType.ShopMenu;
        }
    }

    private void OpenMenu(MenuType menuType, bool priority = false)
    {
        if (!GameManager.MenuPopup.ReadyForInput)
            return; // Don't open a new menu if the popup is currently animating or problems occur

        // Helper method to open a menu of the given type
        GameObject prefabToOpen = GetPrefabForMenuType(menuType);
        if (prefabToOpen != null)
        {
            GameManager.TriggerPopIn(GameManager.MenuPopup, prefabToOpen, forceSwap: priority, onComplete: go =>
            {
                currentMenu = go;
                if (currentMenu == null)
                    Debug.Log($"Failed to trigger {menuType} popup - something may already be open.");
            });
        }
    }

    private void CloseCurrentMenu()
    {
        if (currentMenu != null)
        {
            GameManager.TriggerPopOut(GameManager.MenuPopup);
            currentMenu = null;
            currentMenuType = MenuType.None;
        }
    }

    private GameObject GetPrefabForMenuType(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.None:
                return null;
            case MenuType.MainMenu:
                return menuPrefab;
            case MenuType.UpgradeMenu:
                return upgradeMenuPrefab;
            case MenuType.InventoryMenu:
                return inventoryMenuPrefab;
            case MenuType.ShopMenu:
                return shopMenuPrefab;
            default:
                Debug.LogError($"Unhandled menu type {menuType} in GetPrefabForMenuType!");
                return null;
        }
    }
}