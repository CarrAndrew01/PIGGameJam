using Sirenix.OdinInspector;
using System.Collections.Generic;
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

    public int FishShadowsInScene { get; private set; } = 0;

    [ShowInInspector, ReadOnly] private float nextFishSpawnTime = 0f; // Time at which the next fish should spawn
    [ShowInInspector, ReadOnly] private float fishSpawnTimer = 0f; // Timer to track time since last fish spawn

    // Variables
    [Header("Environment Info")]
    public string environmentName;
    public List<FishCatchInfo> fishTypes; // List of different fish types that can be caught in this environment
    public bool doesSpawnFish = true; // Whether this environment should spawn fish shadows
    public float waterHeight = -3.5f;


    [Header("Fish Spawn Settings")]
    public float minFishSpawnInterval = 30f; // Minimum time between fish spawns (seconds)
    public float maxFishSpawnInterval = 60f; // Maximum time between fish spawns (seconds)
    public float fishSpawnRadius = 5f; // Radius around the environment's position where fish shadows can spawn
    public int maxFishShadows = 50; // Maximum number of fish shadows that can exist at once
    public int initialFishShadows = 10; // Number of fish shadows to spawn when the environment is first loaded


    [Header("Components")]

    [Header("Prefabs")]
    public GameObject fishShadowPrefab; // Prefab for the fish shadow that appears when a fish is caught

    // Properties
    public static string Name => CurrentEnvironment != null ? CurrentEnvironment.environmentName : "Unknown Environment";
    public static float WaterHeight => CurrentEnvironment != null ? CurrentEnvironment.waterHeight : 0f;

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

        GetNextSpawnTime(); // Initialize the next spawn time
    }

    void Start()
    {
        // Spawn some initial fish shadows to populate the environment
        if (doesSpawnFish)
        {
            for (int i = 0; i < initialFishShadows; i++)
            {
                SpawnFishShadow();
            }
        }
    }

    void OnDestroy()
    {
        if (CurrentEnvironment == this)
        {
            CurrentEnvironment = null;
        }
    }

    void Update()
    {
        CheckSpawnFish();
    }

    // Static Methods
    public static void OnFishShadowDestroyed()
    {
        if (CurrentEnvironment != null)
        {
            CurrentEnvironment.FishShadowsInScene = Mathf.Max(0, CurrentEnvironment.FishShadowsInScene - 1);
        }
    }

    // Methods
    private void CheckSpawnFish()
    {
        if (!doesSpawnFish || FishShadowsInScene >= maxFishShadows)
            return;

        fishSpawnTimer += Time.deltaTime;
        if (fishSpawnTimer >= nextFishSpawnTime)
        {
            SpawnFishShadow();
            fishSpawnTimer = 0f;
            GetNextSpawnTime();
        }
    }
    private void GetNextSpawnTime()
    {
        float statFishSpawnInterval = GameManager.GetPlayerStat(StatType.fishSpawnInterval);

        // Fish spawn interval is just a random value between a min and max
        // The player stat acts as a multiplier (1 is normal, 0.5 is half, ect.)
        nextFishSpawnTime = Random.Range(minFishSpawnInterval, maxFishSpawnInterval) * statFishSpawnInterval;
    }
    private void SpawnFishShadow()
    {
        Fish fishToSpawn = GetRandomFish();
        if (fishToSpawn != null)
        {
            // Instantiate the fish shadow prefab at a random position within the environment
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject fishShadowObj = Instantiate(fishShadowPrefab, spawnPosition, Quaternion.identity, transform);
            FishShadowsInScene++;
        }
    }
    private Vector3 GetRandomSpawnPosition()
    {
        // Just grabs a random x offset within spawn radius for spawning
        float randomX = Random.Range(-fishSpawnRadius, fishSpawnRadius);
        return transform.position + new Vector3(randomX, waterHeight, 0f);
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
