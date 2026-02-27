using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Lets the player trigger the fishing minigame for now.
/// </summary>
public class Fishing : MonoBehaviour
{
    public static Fishing Instance; // Singleton instance for easy access

    public static event Action MinigameStarted;
    public static event Action MinigameEnded;

    public static bool CanFish => (IsMinigameActive == false || Instance.IsCharging) && Instance.CurrentBobber == null && !Menus.IsAnyMenuOpen;
    public static bool IsFishing => Instance.CurrentBobber != null;
    public static bool IsMinigameActive => GameManager.MinigamePopup != null && GameManager.MinigamePopup.childCanvas != null;
    public static FishShadow LastFishShadow { get => Instance.lastFishShadow; set => Instance.lastFishShadow = value; }
    public static float reelInFactor = 0f; // Set by the current minigame

    // State
    [Header("State")]
    public bool IsCharging { get; private set; } = false;

    private FishShadow lastFishShadow; // The last fish shadow that was hooked, used to prevent accidentally hooking a new fish shadow when reeling in the current one

    [Header("Variables")]
    public float chargeSpeed = 1.5f; // How long it takes to fully charge the throw, in seconds
    public float ThrowStrength { get; private set; } // How strong the player throws the bobber, which will affect how far it goes. TODO: Make this variable based on how long the player holds the button


    [Header("Input Actions")]
    public InputActionReference fishAction; // expects Button

    [Header("Window Settings")]
    public Vector2 castingPopupPosition = new Vector2(0f, 0f);
    public Vector2 minigamePopupPosition = new Vector2(150f, 0f); // Offset for the minigame popup from the player's position

    [Header("Components")]
    public LineRenderer castLineRenderer; // Point from which the bobber is thrown
    public Bobber CurrentBobber { get; private set; } = null; // The bobber that is currently in the water, if any

    private Transform playerTransform;
    private Transform castPointTransform;
    private ShipMovement shipMovement;

    [Header("Prefabs")]
    public GameObject castingPrefab;
    //public GameObject fishingMinigamePrefab; // Prefab for the fishing minigame popup
    public GameObject bobberPrefab; // Prefab for the bobber that is thrown when fishing


    // ANIMATION
    public static Action<string> OnCast;
    public static Action<string> OnEquip;

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
        shipMovement = playerTransform.GetComponent<ShipMovement>();
        castPointTransform = castLineRenderer.transform;
    }

    void Start()
    {
        // Ensure the line renderer is initially disabled
        castLineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Start charging on press
        if (fishAction.action.WasPressedThisFrame())
        {
            if (CanFish)
            {
                // TRIGGER EQUIP ANIMATION
                OnEquip.Invoke("Equip");

                IsCharging = true;
                ThrowStrength = 0f;

                // Start casting in minigame popup while charging
                GameManager.TriggerPopIn(GameManager.MinigamePopup, castingPrefab, onComplete: go => { }, onBeforeShow: go => { /* no-op during show */ });
            }
        }
        // Charging logic
        if (IsCharging)
        {
            ThrowStrength += Time.deltaTime / chargeSpeed;
            if (ThrowStrength > 1f) ThrowStrength = 1f;
        }
        // Release to throw
        if (IsCharging && fishAction.action.WasReleasedThisFrame())
        {
            // TRIGGER CAST ANIMATION
            OnCast.Invoke("Cast");

            IsCharging = false;
            GameManager.TriggerPopOut(GameManager.MinigamePopup);

            // Debug.Log($"Fish button released, throwing bobber with strength {ThrowStrength}");
            GameObject bobberObj = Instantiate(bobberPrefab, castPointTransform.position, Quaternion.identity);
            CurrentBobber = bobberObj.GetComponent<Bobber>();
            CurrentBobber.Init(castPointTransform, throwStrength: ThrowStrength, direction: shipMovement.LastDirection);
            castLineRenderer.enabled = true; // Enable the line renderer when the bobber is thrown

            ThrowStrength = 0f;
        }
        else if (!CanFish && Instance.CurrentBobber != null && fishAction.action.WasPressedThisFrame())
        {
            // Debug.Log("Fish button pressed while bobber is in water, trying to reel it in");
            if (!IsMinigameActive)
            {
                ReelInCurrentBobber();
            }
            else
            {
                // Debug.Log("Fish button pressed but minigame is already active.");
            }
        }

        // If the bobber is out, add point to line renderer
        if (CurrentBobber != null)
        {
            castLineRenderer.SetPosition(0, castPointTransform.position);
            castLineRenderer.SetPosition(1, CurrentBobber.lineAttachPoint.position);
        }
    }

    // Static methods
    public static void HideCastLine()
    {
        Instance.castLineRenderer.enabled = false;
    }
    public static void ReelInCurrentBobber()
    {
        if (Instance.CurrentBobber != null)
        {
            Instance.CurrentBobber.BeginReelIn();
        }
    }
    public static void StartFishingMinigame(FishShadow fishShadow)
    {
        if (fishShadow != null)
        {
            // Begin fishing
            LastFishShadow = fishShadow;

            // Trigger the fishing minigame popup
            GameObject chosenMinigamePrefab = fishShadow.fishData.fish.minigamePrefab;
            if (chosenMinigamePrefab != null)
            {
                GameManager.TriggerPopIn(GameManager.MinigamePopup, fishShadow.fishData.fish.minigamePrefab, forceSwap: true, onBeforeShow: go =>
                {
                    if (go != null)
                        FindScreenSide();
                });
            }
            else
            {
                // Automatically catch if no minigame prefab is assigned
                LastFishShadow.EndFishing(caught: true);
            }

            MinigameStarted?.Invoke();
        }
    }
    public static void EndFishingMinigame()
    {
        // End fishing
        if (LastFishShadow != null)
        {
            LastFishShadow = null;
        }

        ReelInCurrentBobber();
        reelInFactor = 0f; // Reset the reel in factor for the next catch

        if (IsMinigameActive)
            GameManager.TriggerPopOut(GameManager.MinigamePopup);

        MinigameEnded?.Invoke();
    }

    public static void FindScreenSide()
    {
        // Grab the rect of the minigame popup for later use
        RectTransform minigamePopupRectTransform = GameManager.MinigamePopup.windowRect;

        // Get the player's position on the screen
        Vector3 screenPos = Camera.main.WorldToScreenPoint(Instance.playerTransform.position);
        float screenWidth = Screen.width;

        // Determine which side of the screen the player is on
        if (screenPos.x < screenWidth / 2)
        {
            Debug.Log("Player is on the left side of the screen");
            // Position the minigame popup on the right side
            minigamePopupRectTransform.anchorMin = new Vector2(1f, 0.5f);
            minigamePopupRectTransform.anchorMax = new Vector2(1f, 0.5f);
            minigamePopupRectTransform.pivot = new Vector2(1f, 0.5f); // Match pivot to anchor
            minigamePopupRectTransform.anchoredPosition = new Vector2(-Instance.minigamePopupPosition.x, Instance.minigamePopupPosition.y);
        }
        else
        {
            Debug.Log("Player is on the right side of the screen");
            // Position the minigame popup on the left side
            minigamePopupRectTransform.anchorMin = new Vector2(0f, 0.5f);
            minigamePopupRectTransform.anchorMax = new Vector2(0f, 0.5f);
            minigamePopupRectTransform.pivot = new Vector2(0f, 0.5f); // Match pivot to anchor
            minigamePopupRectTransform.anchoredPosition = Instance.minigamePopupPosition;
        }
    }
}
