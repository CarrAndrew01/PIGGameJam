using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

/// <summary>
/// Singleton manager for game state, progression, etc.
/// </summary>
public class GameManager : MonoBehaviour
{
    // State
    public static GameManager Instance { get; private set; }

    [Header("Player Stats and Inventory")]
    public PlayerStats playerStats = new PlayerStats(); // Manages player upgrades and stats
    public PlayerInventory playerInventory = new PlayerInventory(); // Manages player inventory

    [Header("Screen Transitions")]
    public Transition.Screen intendedScreen;

    [Header("Quests")]
    public List<Quest> AllQuests = new(
    //temp assignments
    );

    public Fish TEMPFISH;

    // Variables
    public const string TITLE_SCENE_NAME = "Title";

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

        // Load persisted player data (upgrades) before applying base stats
        playerStats.LoadUpgradesFromPrefs();
        playerStats.Init();

        // Load inventory after stats and registries are initialized
        playerInventory.LoadFromPlayerPrefs();
        // playerInventory.Init();

        // NOTE: I swear to god, Andrew -- don't leave debug things in and make me think there are bugs.
        // DEBUG_FISH_ADDITION(6);
    }

    void OnApplicationQuit()
    {
        // This is called when the application is quitting. It just saves the player's inventory and upgrades to PlayerPrefs.
        playerInventory.SaveToPlayerPrefs();
        playerStats.SaveUpgradesToPrefs();
    }

    void DEBUG_FISH_ADDITION(int amount)
    {

        for (int i = 0; i < amount; i++)
        {
            CaughtFish newCatch = new CaughtFish(TEMPFISH, Random.Range(TEMPFISH.minWeight, TEMPFISH.maxWeight), "Debug");
            AddFishToInventory(newCatch);
        }
    }

    // Methods
    [ContextMenu("Reapply Upgrades")]
    public void ReapplyUpgrades()
    {
        // This method can be called whenever we want to reapply the effects of all upgrades
        playerStats.ReapplyUpgrades();
    }
    /*
        Sorry for putting this here nathan
    */
    public static float GetNormalizedWeight(float currentWeight, float minWeight, float maxWeight)
    {
        if (maxWeight <= minWeight)
        {
            return 0f; // or throw new ArgumentException("max must be greater than min")
        }

        float normalized = (currentWeight - minWeight) / (maxWeight - minWeight);
        
        // Optional: clamp to [0, 1] to handle values outside the range safely
        return Mathf.Clamp01(normalized);
    }


    // Static methods
    // Upgrades and stats
    public static List<Upgrade> GetPlayerUpgrades() => Instance.playerStats.upgrades;
    public static float GetPlayerStat(StatType statType) => Instance.playerStats.GetStat(statType);
    public static void AddUpgrade(Upgrade upgrade) => Instance.playerStats.AddUpgrade(upgrade);
    public static void RemoveUpgrade(Upgrade upgrade) => Instance.playerStats.RemoveUpgrade(upgrade);

    // Inventory
    public static void AddFishToInventory(CaughtFish newCatch) => Instance.playerInventory.AddFish(newCatch);
    public static void RemoveFishFromInventory(CaughtFish catchToRemove) => Instance.playerInventory.RemoveFish(catchToRemove);
    public static void RemoveFishFromInventoryIndex(int index) => Instance.playerInventory.RemoveFishAt(index);
    public static float CalculateFishValue(CaughtFish fish) => fish.fish.value + (fish.fish.value * GetNormalizedWeight(fish.weight, fish.fish.minWeight, fish.fish.maxWeight));
    public static void AddBaitToInventory(Upgrade baitUpgrade, int uses) => Instance.playerInventory.AddBait(baitUpgrade, uses);
    public static void RemoveBaitFromInventory(Upgrade baitUpgrade, int uses) => Instance.playerInventory.RemoveBait(baitUpgrade, uses);
    public static void SelectBaitUpgrade(Upgrade baitUpgrade) => Instance.playerInventory.SelectBait(baitUpgrade);
    public static void DeselectBaitUpgrade() => Instance.playerInventory.DeselectBait();

    // Popups and Menus
    public static void TriggerPopIn(Popup popup, GameObject canvasPrefab, bool forceSwap = false, System.Action<GameObject> onComplete = null, System.Action<GameObject> onBeforeShow = null) => Instance.StartCoroutine(popup.TriggerPopIn(canvasPrefab, -1f, forceSwap, onComplete, onBeforeShow));
    public static void TriggerPopOut(Popup popup) => popup.TriggerPopOut();
    public static void GotoTitleScreen(Transition.Screen intendedScreen)
    {
        Instance.intendedScreen = intendedScreen;
        UnityEngine.SceneManagement.SceneManager.LoadScene(TITLE_SCENE_NAME);
    }

    // Other
    public static float Money => Instance.playerInventory.money;
    public static void AdjustMoney(float amount) => Instance.playerInventory.money += amount;
    public static void FindQuest(string name) => Instance.AllQuests.Find(quest => quest.questName == name);

    // Asset lookup helpers (search Resources for ScriptableObjects by name)
    public static Fish FindFishTypeByName(string name)
    {
        // Prefer a dedicated Resources subfolder to avoid scanning unrelated assets
        Fish[] all = Resources.LoadAll<Fish>("Fish");
        foreach (var f in all)
            if (f != null && f.name == name)
                return f;
        return null;
    }

    public static Upgrade FindUpgradeByName(string name)
    {
        // Prefer a dedicated Resources subfolder for upgrades; adjust if your assets live elsewhere
        Upgrade[] all = Resources.LoadAll<Upgrade>("Upgrades");
        foreach (var u in all)
            if (u != null && u.name == name)
                return u;
        return null;
    }
}