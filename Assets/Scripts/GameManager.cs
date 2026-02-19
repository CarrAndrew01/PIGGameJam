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

    // Variables
    
    // Components

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
}