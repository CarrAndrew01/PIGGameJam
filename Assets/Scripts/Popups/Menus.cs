using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles opening and closing various menus.
/// </summary>
public class Menus : MonoBehaviour
{
    public static Menus Instance { get; private set; }

    private static bool IsGamePlaying => UnityEngine.Application.isPlaying; // Check if the game is currently running (not in editor mode or paused)
    [ShowInInspector, ReadOnly] public static bool IsAnyMenuOpen => IsGamePlaying && Instance.CurrentMenu != null;

    public enum MenuType
    {
        None,
        MainMenu,
        UpgradeMenu,
        InventoryMenu,
        ShopMenu,
        QuestMenu,
        BaitMenu,
        SettingsMenu
    }

    /// <summary>
    /// This simply lets us specify which menus are openable with flags.
    /// </summary>
    [System.Flags]
    public enum MenuOpenableFlags
    {
        None = 0,
        MainMenu = 1 << 0,
        UpgradeMenu = 1 << 1,
        InventoryMenu = 1 << 2,
        ShopMenu = 1 << 3,
        QuestMenu = 1 << 4,
        BaitMenu = 1 << 5,
        SettingsMenu = 1 << 6,
        All = ~0
    }

    /// <summary>
    /// Lets us turn off specific buttons on the escape menu if we want to.
    /// </summary>
    [System.Flags]
    public enum EscapeMenuButtonFlags
    {
        None = 0,
        Galaxy = 1 << 0,
        Settings = 1 << 1,
        Exit = 1 << 2,
        All = ~0
    }

    [Header("Settings")]
    public MenuOpenableFlags OpenableMenus = MenuOpenableFlags.All; // Which menus can be opened with their respective buttons
    public EscapeMenuButtonFlags EscapeMenuButtons = EscapeMenuButtonFlags.All; // Which buttons should be appear on the escape menu
    public InputActionReference menuAction; // expects Button
    public InputActionReference upgradeMenuAction; // expects Button
    public InputActionReference inventoryMenuAction; // expects Button
    public InputActionReference baitMenuAction; // expects Button

    public InputActionReference shopMenuAction; // expects Button
    public InputActionReference questMenuAction; // expects Button
    public InputActionReference settingsMenuAction; // expects Button

    [Header("Prefabs")]
    public GameObject menuPrefab; // Prefab for the escape menu popup
    public GameObject upgradeMenuPrefab; // Prefab for the upgrade menu popup
    public GameObject inventoryMenuPrefab; // Prefab for the inventory menu popup
    public GameObject shopMenuPrefab; // Prefab for the shop menu popup
    public GameObject questMenuPrefab; // Prefab for the quest menu popup
    public GameObject baitMenuPrefab; // Prefab for the bait menu popup
    public GameObject settingsMenuPrefab; // Prefab for the settings menu popup

    [Header("Debug")]
    public GameObject CurrentMenu { get; private set; } = null; // Reference to the currently open menu, if any
    public MenuType CurrentMenuType { get; private set; } = MenuType.None; // Type of the currently open menu, if any

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

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void OnEnable()
    {
        // Hook into transition events to close menus when transitioning between screens
        Transition.OnTransition += CloseCurrentMenu;
        Fishing.MinigameStarted += CloseCurrentMenu;
    }

    void OnDisable()
    {
        Transition.OnTransition -= CloseCurrentMenu;
        Fishing.MinigameStarted -= CloseCurrentMenu;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.MenuPopup.ReadyForInput || Transition.CurrentScreen == Transition.Screen.Main || Fishing.IsMinigameActive)
            return; // Don't allow menu input if the popup is currently animating or problems occur

        if (menuAction != null && menuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.MainMenu))
        {
            if (CurrentMenu != null)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Only open the main menu if no menu is currently open
                OpenMenu(MenuType.MainMenu, priority: true);
                CurrentMenuType = MenuType.MainMenu;
            }
        }
        else if (upgradeMenuAction != null && upgradeMenuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.UpgradeMenu))
        {
            if (CurrentMenu != null && CurrentMenuType == MenuType.UpgradeMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the upgrade menu
                OpenMenu(MenuType.UpgradeMenu, priority: true);
                CurrentMenuType = MenuType.UpgradeMenu;
            }
        }
        else if (inventoryMenuAction != null && inventoryMenuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.InventoryMenu))
        {
            if (CurrentMenu != null && CurrentMenuType == MenuType.InventoryMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the inventory menu
                OpenMenu(MenuType.InventoryMenu, priority: true);
                CurrentMenuType = MenuType.InventoryMenu;
            }
        }
        // DEBUG ONLY -- These shouldn't be openable with buttons
        else if (shopMenuAction != null && shopMenuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.ShopMenu))
        {
            if (CurrentMenu != null && CurrentMenuType == MenuType.ShopMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the shop menu
                OpenMenu(MenuType.ShopMenu, priority: true);
                CurrentMenuType = MenuType.ShopMenu;
            }
        }
        else if (questMenuAction != null && questMenuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.QuestMenu))
        {
            if (CurrentMenu != null && CurrentMenuType == MenuType.QuestMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the quest menu
                OpenMenu(MenuType.QuestMenu, priority: true);
                CurrentMenuType = MenuType.QuestMenu;
            }
        }
        else if (baitMenuAction != null && baitMenuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.BaitMenu))
        {
            if (CurrentMenu != null && CurrentMenuType == MenuType.BaitMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the bait menu
                OpenMenu(MenuType.BaitMenu, priority: true);
                CurrentMenuType = MenuType.BaitMenu;
            }
        }
        else if (settingsMenuAction != null && settingsMenuAction.action.WasPressedThisFrame() && IsMenuOpenable(MenuType.SettingsMenu))
        {
            if (CurrentMenu != null && CurrentMenuType == MenuType.SettingsMenu)
            {
                CloseCurrentMenu();
            }
            else
            {
                // Otherwise, open the settings menu
                OpenMenu(MenuType.SettingsMenu, priority: true);
                CurrentMenuType = MenuType.SettingsMenu;
            }
        }
    }

    public void TriggerShopMenu()
    {
        if (CurrentMenuType != MenuType.ShopMenu)
        {
            // Open the shop menu, regardless of what is open
            OpenMenu(MenuType.ShopMenu, priority: true);
            CurrentMenuType = MenuType.ShopMenu;
        }
    }

    public void TriggerQuestMenu()
    {
        if (CurrentMenuType != MenuType.QuestMenu)
        {
            // Open the quest menu, regardless of what is open
            OpenMenu(MenuType.QuestMenu, priority: true);
            CurrentMenuType = MenuType.QuestMenu;
        }
    }

    public void TriggerSettingsMenu()
    {
        if (CurrentMenuType != MenuType.SettingsMenu)
        {
            // Open the settings menu, regardless of what is open
            OpenMenu(MenuType.SettingsMenu, priority: true);
            CurrentMenuType = MenuType.SettingsMenu;
        }
    }

    private bool IsMenuOpenable(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.None: return false;
            case MenuType.MainMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.MainMenu);
            case MenuType.UpgradeMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.UpgradeMenu);
            case MenuType.InventoryMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.InventoryMenu);
            case MenuType.ShopMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.ShopMenu);
            case MenuType.QuestMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.QuestMenu);
            case MenuType.BaitMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.BaitMenu);
            case MenuType.SettingsMenu: return OpenableMenus.HasFlag(MenuOpenableFlags.SettingsMenu);
            default: Debug.LogError($"Unhandled menu type {menuType} in IsMenuOpenable!"); return false;
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
            GameManager.TriggerPopIn(GameManager.MenuPopup, prefabToOpen, forceSwap: priority, onBeforeShow: go =>
            {
                CurrentMenu = go;
                if (CurrentMenu == null)
                    Debug.Log($"Failed to trigger {menuType} popup - something may already be open.");
            });
        }
    }

    public void CloseCurrentMenu()
    {
        if (CurrentMenu != null)
        {
            GameManager.TriggerPopOut(GameManager.MenuPopup, onAfter: go =>
            {
                CurrentMenu = null;
                CurrentMenuType = MenuType.None;
            });
        }
    }

    private GameObject GetPrefabForMenuType(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.None: return null;
            case MenuType.MainMenu: return menuPrefab;
            case MenuType.UpgradeMenu: return upgradeMenuPrefab;
            case MenuType.InventoryMenu: return inventoryMenuPrefab;
            case MenuType.ShopMenu: return shopMenuPrefab;
            case MenuType.QuestMenu: return questMenuPrefab;
            case MenuType.BaitMenu: return baitMenuPrefab;
            case MenuType.SettingsMenu: return settingsMenuPrefab;
            default: Debug.LogError($"Unhandled menu type {menuType} in GetPrefabForMenuType!"); return null;
        }
    }
}