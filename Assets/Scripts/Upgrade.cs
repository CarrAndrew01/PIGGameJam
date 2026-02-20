using UnityEngine;

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

    public StatType type;
    public UpgradeModifierType modifierType;
    public float amount; // The amount the upgrade modifies the relevant stat by

    // Methods
    public string GetMechanicalDescription()
    {
        string modifierString = modifierType == UpgradeModifierType.Additive ? $"{amount:+0.##;-0.##;0}" : $"{(amount-1f):+0.##%;-0.##%;0%}";
        return $"{type}: {modifierString}";
    }
}