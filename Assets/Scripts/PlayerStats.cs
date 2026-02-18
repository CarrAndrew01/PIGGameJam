using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Class to set base player stats to then grab from in other scripts.
/// </summary>
[CreateAssetMenu(fileName = "BASE PLAYER STATS", menuName = "Base Player Stats")]
public class BasePlayerStats : ScriptableObject
{
    // Constants
    public static readonly string RESOURCE_PATH = "BASE PLAYER STATS"; // The path in the Resources folder where the PlayerStats asset is
    // Variables
    public List<Stat> stats;

    [Serializable]
    public class Stat
    {
        public string type;
        public float value = 1f;
    }

    // Static methods
    public static BasePlayerStats LoadFromResources()
    {
        BasePlayerStats baseStats = Resources.Load<BasePlayerStats>(RESOURCE_PATH);
        if (baseStats == null)
        {
            Debug.LogError($"PlayerStats resource not found at path {RESOURCE_PATH}! Make sure there's a PlayerStats asset in a Resources folder at that path.");
        }
        return baseStats;
    }
}

/// <summary>
/// Class to manage player stats and upgrades.
/// </summary>
[Serializable]
public class PlayerStats
{
    // State
    public bool hasAppliedUpgrades = false;
    public Dictionary<string, float> currentStats = new Dictionary<string, float>(); // Dictionary to hold the player's current stats
    public Dictionary<string, float> baseStats = new Dictionary<string, float>(); // Dictionary to hold the base values of stats before upgrades, used for calculating the effects of upgrades
    public List<Upgrade> upgrades = new List<Upgrade>();

    public void Init()
    {
        InitializeBaseStats();

        ApplyUpgrades();
    }

    // Methods
    public float GetStat(string statType)
    {
        if (currentStats.TryGetValue(statType, out float value))
            return value;
        else
        {
            Debug.LogWarning($"Trying to get stat of type {statType} which is not in currentStats. Make sure its set somewhere first.");
            return 0f;
        }
    }
    
    public void InitializeBaseStats()
    {
        // Grabs the resource for base player stats
        BasePlayerStats playerStats = BasePlayerStats.LoadFromResources();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats resource not found! Make sure there's a PlayerStats asset in a Resources folder.");
            return;
        }
        // Initializes the baseStats and currentStats dictionaries with the values from the PlayerStats resource
        foreach (BasePlayerStats.Stat stat in playerStats.stats)
        {
            baseStats[stat.type] = stat.value;
            currentStats[stat.type] = stat.value;
        }
    }

    public void ApplyUpgrades()
    {
        if (hasAppliedUpgrades) return;

        // Runs through all upgrades and applies them, used for applying all current upgrades at once (like on game start or when loading a save)
        foreach (Upgrade upgrade in upgrades)
        {
            ApplyUpgrade(upgrade);
        }
        hasAppliedUpgrades = true;
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        // Adds then applies the upgrade
        upgrades.Add(upgrade);

        ApplyUpgrade(upgrade);
    }

    public void ApplyUpgrade(Upgrade upgrade)
    {
        // If the stat doesn't exist in the current stats, warn and initialize
        if (!currentStats.ContainsKey(upgrade.type))
        {
            Debug.LogWarning($"Applying upgrade of type {upgrade.type} which is not in currentStats. Make sure its set somewhere first.");
            float baseValue = baseStats.ContainsKey(upgrade.type) ? baseStats[upgrade.type] : 0f;
            currentStats[upgrade.type] = baseValue;
        }

        // Apply the upgrade based on its modifier type
        switch (upgrade.modifierType)
        {
            case Upgrade.UpgradeModifierType.Additive:
                currentStats[upgrade.type] += upgrade.amount;
                break;
            case Upgrade.UpgradeModifierType.Multiplicative:
                currentStats[upgrade.type] *= (1 + upgrade.amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RemoveUpgrade(Upgrade upgrade)
    {
        // Remove then reverse the upgrade
        if (!upgrades.Remove(upgrade))
        {
            Debug.LogWarning("Trying to remove an upgrade that isn't applied.");
        }
        ReverseUpgrade(upgrade);
    }

    public void ReverseUpgrade(Upgrade upgrade)
    {
        // Undo the upgrade effect
        if (!currentStats.ContainsKey(upgrade.type))
        {
            Debug.LogWarning($"Reversing upgrade of type {upgrade.type} which is not in currentStats. Make sure its set somewhere first.");
            return;
        }

        switch (upgrade.modifierType)
        {
            case Upgrade.UpgradeModifierType.Additive:
                currentStats[upgrade.type] -= upgrade.amount;
                break;
            case Upgrade.UpgradeModifierType.Multiplicative:
                currentStats[upgrade.type] /= (1 + upgrade.amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

/// <summary>
/// Represents an upgrade that can be applied to the player, modifying their stats in some way.
/// </summary>
[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public enum UpgradeModifierType
    {
        Additive, // Adds a flat amount to the relevant stat
        Multiplicative, // Multiplies the relevant stat by (1 + amount), so 0.5 would be +50% and -0.5 would be -50%
    }

    // Variables
    public string description;
    public Sprite icon;

    public string type;
    public UpgradeModifierType modifierType;
    public float amount; // The amount the upgrade modifies the relevant stat by
}