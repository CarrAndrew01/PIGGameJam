using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// Yes I'm aware old way was more flexible. Jes complained.
/// </summary>
public enum StatType
{
    catchSpeed,
    catchArea,
    fishWeight,
    hookGravity,
    fishEscapeChance,
    fishStorage,
}

/// <summary>
/// Class to manage player stats and upgrades.
/// </summary>
[Serializable]
public class PlayerStats
{
    // State
    [ReadOnly]
    public bool hasAppliedUpgrades = false;
    public Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>(); // Dictionary to hold the player's current stats
    public Dictionary<StatType, float> baseStats = new Dictionary<StatType, float>(); // Dictionary to hold the base values of stats before upgrades, used for calculating the effects of upgrades
    public List<Upgrade> upgrades = new List<Upgrade>();

    public void Init()
    {
        InitializeBaseStats();

        ApplyUpgrades();
    }

    // Methods
    public float GetStat(StatType statType)
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
                currentStats[upgrade.type] *= (upgrade.amount);
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
                currentStats[upgrade.type] /= (upgrade.amount);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}