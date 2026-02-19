using UnityEngine;

/// <summary>
/// Struct for holding a fish
/// </summary>
public struct CaughtFish
{
    public Fish fish;
    public float weight;
    public string planetOfOrigin;
}


/// <summary>
/// Represents a type of fish.
/// </summary>
[CreateAssetMenu(fileName = "New Fish", menuName = "Fish")]
public class Fish : ScriptableObject
{
    // Variables
    [Header("General")]
    public Sprite sprite;
    public float minWeight, maxWeight; // Weight range for the fish, which will affect money earned I guess
    public int minAmount = 1, maxAmount = 1; // Amount range for the fish, which will affect how many can be caught in one catch

    [Header("Stardew variables")]
    [Range(0f, 1f)]
    public float jumpiness = 1f;   // feinting or sudden movements
    [Range(0f, 1f)]
    public float speed = 1f;       // how quickly the fish can move
    [Range(0f, 1f)]
    public float stubbornness = 1f;// how unlikely the fish is to change direction
    [Range(0f, 1f)]
    public float size = 1f;        // how big the fish is, which will affect catch area
}
