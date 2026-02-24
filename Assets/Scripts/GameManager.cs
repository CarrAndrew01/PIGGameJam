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

    public int money = 0; // Player's current money, 

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
    }

    // Methods
    [ContextMenu("Reapply Upgrades")]
    public void ReapplyUpgrades()
    {
        // This method can be called whenever we want to reapply the effects of all upgrades
        playerStats.ReapplyUpgrades();
    }

    // Static methods
    public static List<Upgrade> GetPlayerUpgrades() => Instance.playerStats.upgrades;
    public static float GetPlayerStat(StatType statType) => Instance.playerStats.GetStat(statType);
    public static void AddUpgrade(Upgrade upgrade) => Instance.playerStats.AddUpgrade(upgrade);
    public static void AddFishToInventory(CaughtFish newCatch) => Instance.playerInventory.AddFish(newCatch);
    public static void TriggerPopIn(Popup popup, GameObject canvasPrefab, bool forceSwap = false, System.Action<GameObject> onComplete = null, System.Action<GameObject> onBeforeShow = null) => Instance.StartCoroutine(popup.TriggerPopIn(canvasPrefab, -1f, forceSwap, onComplete, onBeforeShow));
    public static void TriggerPopOut(Popup popup) => popup.TriggerPopOut();
    public static void AdjustMoney(int amount) => Instance.money += amount;
}