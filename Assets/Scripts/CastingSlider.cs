using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Literally just a slider that shows the strength of the player's throw when fishing.
/// </summary>
public class CastingSlider : MonoBehaviour
{
    [Header("Components")]
    public Slider slider; // Reference to the UI Slider component

    // Update is called once per frame
    void Update()
    {
        slider.value = Fishing.Instance.ThrowStrength;
    }
}
