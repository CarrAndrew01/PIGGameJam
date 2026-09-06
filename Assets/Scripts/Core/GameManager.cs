using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton manager for game state, progression, etc.
/// </summary>
public class GameManager : MonoBehaviour
{
    // State
    public static GameManager Instance { get; private set; }

    public bool HasSeenIntro { get; set; } = false; // Whether the player has seen the intro cutscene, can be used to skip it on subsequent playthroughs

    [Header("Player Stats and Inventory")]
    public PlayerStats playerStats = new PlayerStats(); // Manages player upgrades and stats
    public PlayerInventory playerInventory = new PlayerInventory(); // Manages player inventory

    [Header("Screen Transitions")]
    public Transition.Screen intendedScreen;

    [Header("Quests")]
    public List<Quest> AllQuests { get; private set; } = new List<Quest>(); // List of all quests in the game, populated at runtime

    // Variables
    public const string TITLE_SCENE_NAME = "Title";
    public const float MIN_DIFFICULTY_MODIFIER = 0.5f;
    public const float MAX_DIFFICULTY_MODIFIER = 1.5f;

    // Components
    [HideInInspector] public Popup minigamePopup; // Reference to the Popup component for minigame pop-ups
    public static Popup MinigamePopup => Instance.minigamePopup; // Static accessor for the minigame popup
    [HideInInspector] public Popup menuPopup; // Reference to the Popup component for handling menus, etc.

    public static Popup MenuPopup => Instance.menuPopup; // Static accessor for the menu popup

    /*****
    DEBUG STUFF just leaving this here as its easier to remember to update it :)
    ****/

    public List<Fish> allFish = new();
    public List<Upgrade> allUpgrades = new();


    public void LoadGalaxyMapDirectly()
    {
        //a version that immediately chucks us into the planets map instead of the title star bit

         
    }

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

        // Get quests from resources
        GetQuestsFromResources();

        // Load persisted player data (upgrades) before applying base stats
        playerStats.LoadUpgradesFromPrefs();
        playerStats.Init();

        // Load inventory after stats so that the max storage limit is applied
        playerInventory.LoadFromPlayerPrefs();
        // playerInventory.Init();
    }

    void OnApplicationQuit()
    {
        // This is called when the application is quitting. It just saves the player's inventory and upgrades to PlayerPrefs.
        playerInventory.SaveToPlayerPrefs();
        playerStats.SaveUpgradesToPrefs();
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

    private void GetQuestsFromResources()
    {
        Quest[] quests = Resources.LoadAll<Quest>("Quests");
        AllQuests.AddRange(quests);
    }

    public Fish GetFish(string fishName)
    {
        foreach(Fish fish in allFish)
        {
            if(fish.name == fishName || fish.fishName == fishName)
            {
                return fish;
            }
        }
        return null;
    }

    public Upgrade GetUpgrade(string upgradeName)
    {
        foreach(Upgrade upgrade in allUpgrades)
        {
            if(upgrade.name == upgradeName || upgrade.upgradeName == upgradeName)
            {
                return upgrade;
            }
        }
        return null;

    }

    //weird little function
    //basically I don't want to have to check for every upgrade and compare its value directly for non-Stardew minigames
    //especially if we add more or change values

    //so this returns the amount of upgrade you get as a simple int (first upgrade is 1, second is 2 etc.) for the logic in the individual minigame to work out
    /// <param name="amountOrLevel">Whether we're counting how many upgrades only (true), or what level they are as well (false)</param>
    public int GetAmountOfUpgrades(string prefix, int upgradeNum = 1, int iterator = 0, bool amountOrLevel = false)
    {
        bool upg = PlayerHasUpgrade(GetUpgrade(prefix + upgradeNum.ToString()));
        
        if (!upg)
        {
            return iterator;           
        }

        return GetAmountOfUpgrades(prefix, upgradeNum + 1, iterator + (amountOrLevel ? 1 : upgradeNum), amountOrLevel);
    }



    // Static methods
    // Saves and loading
    public static void ClearPlayerData()
    {
        Instance.playerInventory.ClearInventory();
        Instance.playerStats.ClearUpgrades();
        PlayerPrefs.DeleteAll();
        Toast.ShowToast("Player data cleared.");
    }
    // Upgrades and stats
    public static List<Upgrade> GetPlayerUpgrades() => Instance.playerStats.upgrades;
    public static bool PlayerHasUpgrade(Upgrade upgrade) => Instance.playerStats.HasUpgrade(upgrade);
    public static float GetPlayerStat(StatType statType) => Instance.playerStats.GetStat(statType);
    public static void AddUpgrade(Upgrade upgrade) => Instance.playerStats.AddUpgrade(upgrade);
    public static void RemoveUpgrade(Upgrade upgrade) => Instance.playerStats.RemoveUpgrade(upgrade);

    // Inventory
    public static void AddFishToInventory(CaughtFish newCatch, bool ignoreLimit = false) => Instance.playerInventory.AddFish(newCatch, ignoreLimit);
    public static void RemoveFishFromInventory(CaughtFish catchToRemove) => Instance.playerInventory.RemoveFish(catchToRemove);
    public static void RemoveFishFromInventoryIndex(int index) => Instance.playerInventory.RemoveFishAt(index);
    public static float CalculateFishValue(CaughtFish fish) => (fish.fish.value + (fish.fish.value * GetNormalizedWeight(fish.weight, fish.fish.minWeight, fish.fish.maxWeight))) * fish.valueMultiplier;
    public static void AddBaitToInventory(Upgrade baitUpgrade, int uses) => Instance.playerInventory.AddBait(baitUpgrade, uses);
    public static void RemoveBaitFromInventory(Upgrade baitUpgrade, int uses) => Instance.playerInventory.RemoveBait(baitUpgrade, uses);
    public static void SelectBaitUpgrade(Upgrade baitUpgrade) => Instance.playerInventory.SelectBait(baitUpgrade);
    public static void DeselectBaitUpgrade() => Instance.playerInventory.DeselectBait();
    public static bool IsInventoryFull() => Instance.playerInventory.caughtFish.Count >= Instance.playerInventory.MaxFishStorage;
    public static float GetDifficultyModifier() => Instance.playerInventory.difficultyModifier;
    public static float GetDifficultyModifierNormalized(float sliderMin = -1f, float sliderMax = -1f)
    {
        // Convert the current difficulty modifier to a 0-1 range for slider representation
        float normalized = Mathf.InverseLerp(MIN_DIFFICULTY_MODIFIER, MAX_DIFFICULTY_MODIFIER, GetDifficultyModifier());

        // If sliderMin and sliderMax are set to valid values, remap the normalized value to that range instead (useful if the slider doesn't use a 0-1 range)
        if (sliderMin >= 0f && sliderMax > sliderMin)
        {
            normalized = Mathf.Lerp(sliderMin, sliderMax, normalized);
        }
        return normalized;
    }
    public static float GetDifficultyModifierNormalized(Slider slider) => GetDifficultyModifierNormalized(slider.minValue, slider.maxValue);
    public static void SetDifficultyModifier(float modNormal)
    {
        // Convert from 0-1 slider range to the defined difficulty modifier range
        float modifier = Mathf.Lerp(MIN_DIFFICULTY_MODIFIER, MAX_DIFFICULTY_MODIFIER, modNormal);

        Instance.playerInventory.difficultyModifier = modifier;
    }
    // Popups and Menus
    public static void TriggerPopIn(Popup popup, GameObject canvasPrefab, bool forceSwap = false, System.Action<GameObject> onComplete = null, System.Action<GameObject> onBeforeShow = null) => Instance.StartCoroutine(popup.TriggerPopIn(canvasPrefab, -1f, forceSwap, onComplete, onBeforeShow));
    public static void TriggerPopOut(Popup popup, System.Action<GameObject> onAfter = null, System.Action<GameObject> onBefore = null, float durationOverride = -1f) => Instance.StartCoroutine(popup.TriggerPopOut(durationOverride, onAfter, onBefore));
    public static void GotoTitleScreen(Transition.Screen intendedScreen)
    {
        Instance.intendedScreen = intendedScreen;
        if (TransitionManager.Instance != null) {
            TransitionManager.Instance.BeginSceneTransition(TITLE_SCENE_NAME, true);
        } else { 
            UnityEngine.SceneManagement.SceneManager.LoadScene(TITLE_SCENE_NAME);
        }
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