using System.Collections.Generic;
using NUnit;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [System.Serializable]
    public struct FishCatchInfo
    {
        public Fish fish;
        public float chance; // Acts as a weight for random selection
    }

    // State
    public static Environment CurrentEnvironment { get; private set; }

    // Variables
    [Header("Environment Info")]
    public string environmentName;
    public List<FishCatchInfo> fishTypes; // List of different fish types that can be caught in this environment

    // Properties
    public static string Name => CurrentEnvironment != null ? CurrentEnvironment.environmentName : "Unknown Environment";

    void Awake()
    {
        // Singleton pattern
        if (CurrentEnvironment != null && CurrentEnvironment != this)
        {
            Debug.LogWarning($"Multiple Environment instances detected! Current: {CurrentEnvironment.environmentName}, New: {environmentName}. Overwriting current environment.");
            Destroy(gameObject);
            return;
        }
        CurrentEnvironment = this;
    }

    void OnDestroy()
    {
        if (CurrentEnvironment == this)
        {
            CurrentEnvironment = null;
        }
    }

    // Static Methods
    public static Fish GetRandomFish()
    {
        if (CurrentEnvironment.fishTypes == null || CurrentEnvironment.fishTypes.Count == 0)
        {
            Debug.LogError($"No fish types defined for environment {CurrentEnvironment.environmentName}!");
            return null;
        }

        // Determine total weight for random selection
        float totalWeight = 0f;
        foreach (FishCatchInfo fishInfo in CurrentEnvironment.fishTypes)
        {
            totalWeight += fishInfo.chance;
        }

        // Select a random fish based on weights
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        foreach (FishCatchInfo fishInfo in CurrentEnvironment.fishTypes)
        {
            cumulativeWeight += fishInfo.chance;
            if (randomValue <= cumulativeWeight)
            {
                return fishInfo.fish;
            }
        }

        Debug.LogError($"Failed to select a random fish in environment {CurrentEnvironment.environmentName}!");
        return null;
    }
}
