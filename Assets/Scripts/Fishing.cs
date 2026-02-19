using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lets the player trigger the fishing minigame for now.
/// </summary>
public class Fishing : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionReference fishAction; // expects Button

    // Components
    [Header("Components")]
    public GameObject fishingMinigamePrefab; // Prefab for the fishing minigame popup

    // Update is called once per frame
    void Update()
    {
        if (fishAction.action.WasPressedThisFrame())
        {
            Debug.Log("Fish button pressed, triggering minigame popup");
            // Trigger the fishing minigame popup
            GameManager.TriggerPopIn(GameManager.MinigamePopup, fishingMinigamePrefab);
        }
    }
}
