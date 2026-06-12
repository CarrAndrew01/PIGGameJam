using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Struct for holding a fish
/// </summary>
[Serializable]
public struct CaughtFish
{
    public int id;
    public Fish fish;
    public float weight;
    public string planetOfOrigin;

    public float valueMultiplier;//extra variable for some fish that might be worth more for whatever reason

    public CaughtFish(Fish fish, float weight, string planetOfOrigin, float valueMultiplier = 1f)
    {
        // Do not assign an inventory ID here. IDs are assigned when a fish is added to inventory
        // Otherwise we count really high just with shadows
        id = 0;
        this.fish = fish;
        this.weight = weight;
        this.planetOfOrigin = planetOfOrigin;
        this.valueMultiplier = valueMultiplier;
    }

}


/// <summary>
/// Represents a type of fish.
/// </summary>
[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class Fish : ScriptableObject
{
    // Variables
    [Header("General")]
    public string fishName;
    [TextArea(2, 5)]
    public string description;

    [Header("Sprites")]
    public Sprite sprite;
    public Sprite WithBackground; //sprites with an in-built background
    public Sprite background; //background sprite if it doesn't come with one (if we use this idk)

    [Header("Variables")]
    public float value;
    public float minWeight, maxWeight; // Weight range for the fish, which will affect money earned I guess
    public int minAmount = 1, maxAmount = 1; // Amount range for the fish, which will affect how many can be caught in one catch

    [Header("Bait and Summoning")]
    public int preferredBaitType; // A specific number that must be matched by the baitType stat in order to bite, 0 for no preference
    public float minSummonTime = 20f, maxSummonTime = 60f; // Time range for how long it takes the fish to be summoned with correct bait

    public bool tempBoolForPokemon = true;

    [Header("Stardew variables")]
    [Range(0f, 1f)]
    [Tooltip("How much random movement a fish has when struggling (standing still).")]
    [SerializeField] private float jumpiness = 1f;   // feinting or sudden movements
    [Range(0f, 1f)]
    [Tooltip("How quickly the fish reaches max velocity.")]
    [SerializeField] private float speed = 1f;       // how quickly the fish can move
    [Range(0f, 1f)]
    [Tooltip("How likely the fish is to change direction when struggling.")]
    [SerializeField] private float stubbornness = 1f;// how unlikely the fish is to change direction
    [Range(0f, 1f)]
    [Tooltip("How much momentum the fish has (slows down slower).")]
    [SerializeField] private float size = 1f;        // how big the fish is

    [Header("Modifiers")]
    [Tooltip("Multiplier for how much the fish struggles (bigger is more struggle)")]
    [SerializeField] private float timeToCatchMult = 1f; // Multiplier for how long it takes to catch the fish
    [Tooltip("Multiplier for how long it takes for the fish to escape (bigger is longer)")]
    [SerializeField] private float timeToEscapeMult = 1f; // Multiplier for how long it takes for the fish to escape

    [Header("Prefabs")]
    public GameObject minigamePrefab;

    // Access properties
    public float Jumpiness => jumpiness * GameManager.GetDifficultyModifier();
    public float Speed => speed * GameManager.GetDifficultyModifier();
    public float Stubbornness => stubbornness * GameManager.GetDifficultyModifier();
    public float Size => size * GameManager.GetDifficultyModifier();

    public float TimeToCatchMult => timeToCatchMult * GameManager.GetDifficultyModifier();
    public float TimeToEscapeMult => timeToEscapeMult / GameManager.GetDifficultyModifier();
}
