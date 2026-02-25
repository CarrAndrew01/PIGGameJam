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
    //We just have 1 list of quests, as there won't be enough to be an issue, 
    //if extended, you'd have different lists for completed and active quests, but as thered probably <30 total its much easier to just have one list
    //and the completion information inside Quest.cs
    
    public List<Quest> AllQuests = new(
        //temp assignments
    ); 

    public Fish TEMPFISH;

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
        // AddFishToInventory(new CaughtFish(TEMPFISH, 10f, "123"));
        // AddFishToInventory(new CaughtFish(TEMPFISH, 10f, "123"));
        // AddFishToInventory(new CaughtFish(TEMPFISH, 10f, "123"));
        // //TEMP REMOVE THIS LATER, JUST FOR TESTING
        // AddFishToInventory(new CaughtFish(TEMPFISH, 10f, "123"));
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
    public static void FindQuest(string name) => Instance.AllQuests.Find(quest => quest.questName == name);
}