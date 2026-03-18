using System.Text.RegularExpressions;
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
    public string upgradeName;
    [TextArea(2, 5)]
    public string description;
    [TextArea(2, 5)]
    [Tooltip("Left empty, it will use the function description instead.")]
    public string mechDescriptionOverride; // Optional field
    public Sprite icon;

    public StatType type;
    public UpgradeModifierType modifierType;
    public float amount; // The amount the upgrade modifies the relevant stat by

    // Methods
    public string GetMechanicalDescription()
    {
        if (!string.IsNullOrEmpty(mechDescriptionOverride))
        {
            return mechDescriptionOverride;
        }
        string modifierString = modifierType == UpgradeModifierType.Additive ? $"{amount:+0.##;-0.##;0}" : $"{(amount - 1f):+0.##%;-0.##%;0%}";
        // Insert spaces into camelCase / PascalCase enum names (e.g. fishStorage -> fish Storage)
        string rawName = type.ToString();
        // Add space between lower/number and upper, and between letters and numbers
        rawName = Regex.Replace(rawName, "([a-z0-9])([A-Z])", "$1 $2");
        rawName = Regex.Replace(rawName, "([A-Za-z])([0-9])", "$1 $2");
        return $"{rawName}: {modifierString}";
    }
}