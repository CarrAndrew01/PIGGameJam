using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Singleton manager for game state, progression, etc.
/// </summary>
public class GameManager : MonoBehaviour
{
    // State
    public static GameManager Instance { get; private set; }

    [Header("Player Stats and Upgrades")]
    public PlayerStats playerStats = new PlayerStats(); // Manages player upgrades and stats

    public PlayerInventory playerInventory = new PlayerInventory(); // Manages player inventory

    // Variables
    
    // Components
    [HideInInspector] public Popup minigamePopup; // Reference to the Popup component for minigame pop-ups
    public static Popup MinigamePopup => Instance.minigamePopup; // Static accessor for the minigame popup
    [HideInInspector] public Popup menuPopup; // Reference to the Popup component for handling menus, etc.
    public static Popup MenuPopup => Instance.menuPopup; // Static accessor for the menu popup

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Setup
        playerStats.Init();
        playerInventory.Init();
    }

    // Methods

    // Static methods
    public static List<Upgrade> GetPlayerUpgrades() => Instance.playerStats.upgrades;
    public static float GetPlayerStat(StatType statType) => Instance.playerStats.GetStat(statType);
    public static void AddFishToInventory(CaughtFish newCatch) => Instance.playerInventory.AddFish(newCatch);
    public static GameObject TriggerPopIn(Popup popup, GameObject canvasPrefab) => popup.TriggerPopIn(canvasPrefab);
    public static void TriggerPopOut(Popup popup) => popup.TriggerPopOut();
    public static IEnumerator TriggerSwap(Popup popup, GameObject newMenuPrefab, System.Action<GameObject> onSwapComplete = null)
    {
        // This needs to be a coroutine because we need to wait for the pop-out animation to finish before starting the pop-in animation, otherwise they will conflict and cause bugs
        yield return popup.StartCoroutine(popup.TriggerSwap(newMenuPrefab, onSwapComplete));
    }
}