using System.Collections.Generic;
using UnityEngine;

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

    public Transition.IntendedScreen intendedScreen;

    // Variables
    public const string TITLE_SCENE_NAME = "Title";
    
    // Components
    [HideInInspector] public Popup minigamePopup; // Reference to the Popup component for minigame pop-ups
    public static Popup MinigamePopup => Instance.minigamePopup; // Static accessor for the minigame popup
    [HideInInspector] public Popup menuPopup; // Reference to the Popup component for handling menus, etc.

    public static Popup MenuPopup => Instance.menuPopup; // Static accessor for the menu popup

    public int money = 0; // Player's current money, 

    /*****
    Quest stuff here
    ********/
    //master list containing every single quest. no information abotu completion is contained here sohis is NOT accessed directly except to find and copy quests to the other 2 lists 
    // (if we need a list of inactive quests I'll make it)
    public List<Quest> ActiveQuests = new(); //quests we've unlocked
    public List<Quest> AllQuests = new(); 
    public List<Quest> CompletedQuests = new(); 



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

    public static void GotoTitleScreen(Transition.IntendedScreen intendedScreen)
    {
        Instance.intendedScreen = intendedScreen;
        UnityEngine.SceneManagement.SceneManager.LoadScene(TITLE_SCENE_NAME);
    }
}