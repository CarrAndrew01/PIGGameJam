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

    public void ReapplyUpgrades()
    {
        // Resets stats back to base values, then reapplies all upgrades. Used for testing and if we want to implement stat respec or temporary stat changes in the future
        ResetStats();
        ApplyUpgrades();
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        // Adds then applies the upgrade
        upgrades.Add(upgrade);

        ReapplyUpgrades();
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

    private void ResetStats()
    {
        // Resets current stats back to base values, used for testing and if we want to implement stat respec or temporary stat changes in the future
        foreach (KeyValuePair<StatType, float> entry in baseStats)
        {
            currentStats[entry.Key] = entry.Value;
        }
        hasAppliedUpgrades = false;
    }

    private void ApplyUpgrades()
    {
        if (hasAppliedUpgrades) return;

        List<Upgrade> additiveUpgrades = new List<Upgrade>();
        List<Upgrade> multiplicativeUpgrades = new List<Upgrade>();

        // First apply all additive upgrades, then multiplicative upgrades
        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.modifierType == Upgrade.UpgradeModifierType.Additive)
                additiveUpgrades.Add(upgrade);
            else if (upgrade.modifierType == Upgrade.UpgradeModifierType.Multiplicative)
                multiplicativeUpgrades.Add(upgrade);
        }
        foreach (Upgrade upgrade in additiveUpgrades)
        {
            ApplyUpgrade(upgrade);
        }
        foreach (Upgrade upgrade in multiplicativeUpgrades)
        {
            ApplyUpgrade(upgrade);
        }
        hasAppliedUpgrades = true;
    }

    private void ApplyUpgrade(Upgrade upgrade)
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

    private void ReverseUpgrade(Upgrade upgrade)
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