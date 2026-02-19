using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Handles the fishing minigame logic inspired by Stardew Valley's fishing.
/// </summary>
public class Stardew : MonoBehaviour
{
    /// <summary>
    /// FishState enum represents the different states the fish can be in during the stardew fishing minigame.
    /// </summary>
    private enum FishState
    {
        Up,
        Down,
        Struggling,
        Caught,
        Escaped
    }

    // Constants
    public static readonly string DEFAULT_FISH_RESOURCE_PATH = "DEFAULT FISH"; // The path in the Resources folder where the default Fish is

    // State
    [Header("Fish State")]
    [ShowInInspector, ReadOnly] private bool isCatching = false;
    [ShowInInspector, ReadOnly] private bool isReeling = false; // whether the player is currently pressing the reel button to move the catch slider up
    [ShowInInspector, ReadOnly] private FishState fishState = FishState.Struggling; // Whether the fish is currently up, down, or struggling
    [ShowInInspector, ReadOnly, Range(-1f, 1f)]
    private float caughtProgress = 0f;
    private float timeSinceStateChange = 0f; // How much time has passed since the fish last changed state
    private float currentStateDuration = 0f; // How long the fish will stay in its current state
    private float fishStateStartPosition = 0f; // Fish slider position when current state began
    private float currentMaxMoveDistance = 0f; // Max distance the fish can travel in the current Up/Down state
    private float currentFishVelocity = 0f; // How quickly the fish slider is currently moving, positive is up, negative is down
    private float currentHookVelocity = 0f; // ^ but for the catch slider

    // Variables
    [Header("Catching Settings")]
    public float catchSize = 200f; // Size of the catch area in pixels
    public float catchRate = 0.1f; // How quickly the player catches the fish when succesfully catching
    public float escapeRate = 0.2f; // How quickly the fish escapes when the player is not successfully catching
    public float hookAcceleration = 1f; // How quickly the hook accelerates upward when reeling
    public float hookGravity = 0.5f; // Downward acceleration applied when not reeling
    public float hookMaxVelocity = 2f; // Maximum velocity the hook can reach
    public Color catchSuccessColor = Color.green; // Color of the success slider when the catch is successful
    public Color catchNeutralColor = Color.cyan; // Color of the success slider when the catch is neutral (neither successful nor unsuccessful)
    public Color catchFailureColor = Color.red; // Color of the success slider when the catch is unsuccessful
    public Color hookColor = Color.gray; // Color of the catch slider handle (the hook)

    // Catch properties that take into account player stats and upgrades
    public float CatchRate => catchRate * statCatchSpeed;
    public float EscapeRate => escapeRate * statFishEscapeRate;
    public float HookGravity => hookGravity * statHookGravity;
    public float CatchAreaSize => catchSize * statCatchArea;

    [Header("Fish Settings")]
    public float fishMaxVelocity = 1.5f; // Maximum velocity the fish can reach
    public float fishMinMoveDistance = 0.05f; // Minimum distance the fish moves in one direction before struggling
    public float fishMaxMoveDistance = 0.4f; // Maximum distance the fish moves in one direction before struggling
    public float jumpinessMult = 1f; // Multiplier for how twitchy the fish is
    public float speedMult = 1f; // Multiplier for how quickly the fish accelerates
    public float stubbornnessMult = 1f; // Multiplier for how unlikely the fish is to change direction
    public float sizeMult = 1f; // Multiplier for how big the fish is, which will affect catch area

    // Fish properties which take into account modifers
    public float Jumpiness => fish.jumpiness * jumpinessMult;
    public float Speed => fish.speed * speedMult;
    public float Stubbornness => fish.stubbornness * stubbornnessMult;
    public float Size => fish.size * sizeMult;

    private CaughtFish caughtFish;
    private int amountInCatch = 1; // determines if extra fish are added if we successfully catch

    // Player stats that affect minigame
    private float statCatchSpeed, statCatchArea, statFishWeight, statHookGravity, statFishEscapeRate;

    // Input actions
    [Header("Input Actions")]
    public InputActionReference reelAction; // button input to reel the hook upward

    // Components
    [Header("Components")]
    public Slider fishSlider; // Reference to the Slider component used for showing the fish location
    public Slider catchSlider; // Reference to the Slider component used for showing the catch progress
    public Slider successSlider; // Reference to Slider for success rate
    public Image fishImage; // Reference to the Image component of the slider handle
    public Image catchImage; // ^ but for the catch slider
    public Image successImage; // Reference to the fill area of the success slider, which changes color based on success
    [HideInInspector] public Fish fish; // Reference to the fish ScriptableObject, set when we create the Stardew instance in the scene

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grab player stats from GameManager
        statCatchSpeed = GameManager.GetPlayerStat(StatType.catchSpeed);
        statCatchArea = GameManager.GetPlayerStat(StatType.catchArea);
        statFishWeight = GameManager.GetPlayerStat(StatType.fishWeight);
        statHookGravity = GameManager.GetPlayerStat(StatType.hookGravity);
        statFishEscapeRate = GameManager.GetPlayerStat(StatType.fishEscapeChance);

        // Grab a random fish from the current environment
        fish = Environment.GetRandomFish();
        if (fish == null)
        {
            // Grab default fish from Resources if not set for some reason
            Debug.LogWarning("No fish found for current environment, loading default fish from Resources.");
            fish = Resources.Load<Fish>(DEFAULT_FISH_RESOURCE_PATH);
        }

        // Assign the fish sprite to the UI image
        if (fishImage != null && fish != null)
            fishImage.sprite = fish.sprite;

        // Initialize instance-specific values
        caughtFish = new CaughtFish()
        {
            fish = fish,
            weight = Random.Range(fish.minWeight, fish.maxWeight) * statFishWeight,
            planetOfOrigin = Environment.Name
        };
        amountInCatch = Random.Range(fish.minAmount, fish.maxAmount + 1); // +1 because Random.Range is exclusive of the upper bound

        RectTransform hookRect = catchImage.GetComponent<RectTransform>();
        hookRect.sizeDelta = new Vector2(hookRect.sizeDelta.x, CatchAreaSize);
        catchImage.color = hookColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (fishState == FishState.Caught || fishState == FishState.Escaped) return; // If we've already caught or escaped the fish, no need to update anything

        // Player input and hook movement
        GetInput();
        ApplyHookGravity();
        ApplyHookVelocity();

        FishState newFishState = DecideFishState();

        if (this.fishState != newFishState)
        { // If state has changed, set the new state and begin a new timer
            this.fishState = newFishState;
            timeSinceStateChange = 0f;
            currentStateDuration = Random.Range(1f, 3f) * (1f + Stubbornness);
            fishStateStartPosition = fishSlider.value;
            currentMaxMoveDistance = Random.Range(fishMinMoveDistance, fishMaxMoveDistance);
        }

        UpdateFishMovement();

        // Then, update the caught progress based on whether the player is currently filling the catch slider or not
        UpdateCaughtProgress();
    }

    private void GetInput()
    {
        isReeling = reelAction.action.IsPressed();

        if (isReeling)
        {
            // Accelerate the hook upward
            currentHookVelocity += hookAcceleration * Time.deltaTime;
        }
    }

    private void ApplyHookGravity()
    {
        if (!isReeling)
        {
            // Gravity decelerates / pulls hook downward
            currentHookVelocity -= HookGravity * Time.deltaTime;
        }
    }

    private void ApplyHookVelocity()
    {
        currentHookVelocity = Mathf.Clamp(currentHookVelocity, -hookMaxVelocity, hookMaxVelocity);
        catchSlider.value += currentHookVelocity * Time.deltaTime;

        // Stop velocity at slider bounds
        if (catchSlider.value <= 0f || catchSlider.value >= 1f)
        {
            currentHookVelocity = 0f;
        }

        catchSlider.value = Mathf.Clamp01(catchSlider.value);
    }

    private FishState DecideFishState()
    {
        timeSinceStateChange += Time.deltaTime;

        if (timeSinceStateChange >= currentStateDuration)
        {
            float pos = fishSlider.value; // 0 = bottom, 1 = top

            // Weight for Up decreases as fish approaches top (0 at pos=1)
            float upWeight = 1f - pos;
            // Weight for Down decreases as fish approaches bottom (0 at pos=0)
            float downWeight = pos;
            // Struggling is always equally likely
            float struggleWeight = 1f;

            float total = upWeight + downWeight + struggleWeight;
            float roll = Random.Range(0f, total);

            if (roll < upWeight)
                return FishState.Up;
            else if (roll < upWeight + downWeight)
                return FishState.Down;
            else
                return FishState.Struggling;
        }

        return this.fishState; // No change in state
    }

    private void UpdateFishMovement()
    {
        float fishAcceleration = Speed;

        // If the fish has moved past its max distance, force it into Struggling
        if (fishState == FishState.Up || fishState == FishState.Down)
        {
            float distanceTraveled = Mathf.Abs(fishSlider.value - fishStateStartPosition);
            if (distanceTraveled >= currentMaxMoveDistance)
            {
                fishState = FishState.Struggling;
                timeSinceStateChange = 0f;
                currentStateDuration = Random.Range(0.5f, 1.5f) * (1f + Stubbornness);
            }
        }

        switch (fishState)
        {
            case FishState.Up:
                currentFishVelocity += fishAcceleration * Time.deltaTime;
                break;
            case FishState.Down:
                currentFishVelocity -= fishAcceleration * Time.deltaTime;
                break;
            case FishState.Struggling:
                // Decelerate based on Weight (heavier fish slow down slower due to momentum)
                float deceleration = fishAcceleration / (1f + Size);
                currentFishVelocity = Mathf.MoveTowards(currentFishVelocity, 0f, deceleration * Time.deltaTime);

                // Wriggle randomly based on Jumpiness
                float wriggleStrength = fishMaxVelocity * 0.3f * Jumpiness;
                currentFishVelocity += Random.Range(-wriggleStrength, wriggleStrength) * Time.deltaTime * 10f * Jumpiness;
                break;
        }

        currentFishVelocity = Mathf.Clamp(currentFishVelocity, -fishMaxVelocity, fishMaxVelocity);
        fishSlider.value += currentFishVelocity * Time.deltaTime;

        // Stop velocity at slider bounds
        if (fishSlider.value <= 0f || fishSlider.value >= 1f)
        {
            currentFishVelocity = 0f;
        }

        fishSlider.value = Mathf.Clamp01(fishSlider.value);
    }

    private void IsInCatchArea()
    {
        float fishValue = fishSlider.value;
        float catchValue = catchSlider.value;

        RectTransform hookRect = catchImage.GetComponent<RectTransform>();
        RectTransform sliderRect = catchSlider.GetComponent<RectTransform>();
        float catchHalfSizeNormalized = (hookRect.sizeDelta.y / sliderRect.rect.height) / 2f;

        isCatching = Mathf.Abs(fishValue - catchValue) <= catchHalfSizeNormalized;
    }

    private void UpdateCaughtProgress()
    {
        IsInCatchArea();

        if (isCatching)
        {
            caughtProgress += CatchRate * Time.deltaTime;
        }
        else
        {
            caughtProgress -= EscapeRate * Time.deltaTime;
        }

        caughtProgress = Mathf.Clamp(caughtProgress, -1f, 1f); // Clamp to either -1 (fully escaped) or 1 (fully caught)

        // Check for win/lose conditions
        if (caughtProgress >= 1f)
        {
            fishState = FishState.Caught;
            Debug.Log("Fish Caught!");
            
            // Trigger any catch animations or logic here
            GameManager.AddFishToInventory(caughtFish);
            for (int i = 0; i < amountInCatch - 1; i++)
            {
                // Create additional caught fish for any extra amount
                CaughtFish extraFish = new CaughtFish()
                {
                    fish = fish,
                    weight = Random.Range(fish.minWeight, fish.maxWeight) * statFishWeight,
                    planetOfOrigin = Environment.Name
                };
                GameManager.AddFishToInventory(extraFish);
            }
            Popup.TriggerPopOut();
        }
        else if (caughtProgress <= -1f)
        {
            fishState = FishState.Escaped;
            Debug.Log("Fish Escaped!");
            // Trigger any escape animations or logic here
            Popup.TriggerPopOut();
        }

        // Finally, update the success slider to show how close the player is to catching the fish
        float successValue = Mathf.InverseLerp(-1f, 1f, caughtProgress); // Convert caughtProgress to a 0-1 range for the slider
        successSlider.value = successValue;
        if (caughtProgress > 0f)
            successImage.color = Color.Lerp(catchNeutralColor, catchSuccessColor, caughtProgress);

        else
            successImage.color = Color.Lerp(catchNeutralColor, catchFailureColor, -caughtProgress);
    }
}
