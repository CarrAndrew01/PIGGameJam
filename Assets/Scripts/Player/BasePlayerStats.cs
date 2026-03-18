using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Class to set base player stats to then grab from in other scripts.
/// </summary>
[CreateAssetMenu(fileName = "BASE PLAYER STATS", menuName = "Base Player Stats")]
public class BasePlayerStats : ScriptableObject
{
    // Constants
    public static readonly string RESOURCE_PATH = "BASE PLAYER STATS"; // The path in the Resources folder where the PlayerStats asset is
    // State
    public List<Stat> stats;

    [Serializable]
    public class Stat
    {
        public StatType type;
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