using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lets the player trigger the fishing minigame for now.
/// </summary>
public class Fishing : MonoBehaviour
{
    public static Fishing Instance; // Singleton instance for easy access

    // State
    [Header("State")]
    [ShowInInspector, ReadOnly] private bool playerInRange = false; // Whether the player is currently in range to fish
    [ReadOnly] public FishShadow currentFishShadow; // The fish shadow the player is currently trying to catch, if any
    public bool CanFish => playerInRange && currentFishShadow != null;
    public CaughtFish FishToCatch => currentFishShadow.fishData;

    [Header("Input Actions")]
    public InputActionReference fishAction; // expects Button

    [Header("Window Settings")]
    public Vector3 minigamePopupPosition = new Vector2(150f, 0f); // Offset for the minigame popup from the player's position

    // Components
    [Header("Components")]
    public GameObject fishingMinigamePrefab; // Prefab for the fishing minigame popup

    private Transform playerTransform;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            Debug.Log("Player entered fish trigger");
            playerInRange = true;
            currentFishShadow = other.GetComponent<FishShadow>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            Debug.Log("Player exited fish trigger");
            playerInRange = false;
            currentFishShadow = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fishAction.action.WasPressedThisFrame())
        {
            if (CanFish)
            {
                if (currentFishShadow.IsEscaping)
                {
                    Debug.Log("Tried to fish but the fish is already escaping");
                    return;
                }

                Debug.Log("Fish button pressed while within range, triggering minigame popup");
                // Pause the fish's leave timer while the minigame is active
                currentFishShadow.BeginFishing();

                // Trigger the fishing minigame popup
                GameManager.TriggerPopIn(GameManager.MinigamePopup, fishingMinigamePrefab);
                FindScreenSide();
            }
            else
            {
                Debug.Log("Fish button pressed but player is not in range or there is no fish shadow");
            }
        }
    }

    void FindScreenSide()
    {
        // Grab the rect of the minigame popup for later use
        RectTransform minigamePopupRectTransform = GameManager.MinigamePopup.windowRect;

        // Get the player's position on the screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerTransform.position);
        float screenWidth = Screen.width;

        // Determine which side of the screen the player is on
        if (screenPos.x < screenWidth / 2)
        {
            Debug.Log("Player is on the left side of the screen");
            // Position the minigame popup on the right side
            minigamePopupRectTransform.anchorMin = new Vector2(1f, 0.5f);
            minigamePopupRectTransform.anchorMax = new Vector2(1f, 0.5f);
            minigamePopupRectTransform.pivot = new Vector2(1f, 0.5f); // Match pivot to anchor
            minigamePopupRectTransform.anchoredPosition = new Vector2(-minigamePopupPosition.x, minigamePopupPosition.y);
        }
        else
        {
            Debug.Log("Player is on the right side of the screen");
            // Position the minigame popup on the left side
            minigamePopupRectTransform.anchorMin = new Vector2(0f, 0.5f);
            minigamePopupRectTransform.anchorMax = new Vector2(0f, 0.5f);
            minigamePopupRectTransform.pivot = new Vector2(0f, 0.5f); // Match pivot to anchor
            minigamePopupRectTransform.anchoredPosition = minigamePopupPosition;
        }
    }
}
