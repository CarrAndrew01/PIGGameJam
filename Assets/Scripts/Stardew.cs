using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum FishState
{
    Up,
    Down,
    Struggling,
    Caught,
    Escaped
}

public class Stardew : MonoBehaviour
{
    // State
    [Header("Fish State")]
    [ShowInInspector, ReadOnly] private bool isCatching = false;
    [ShowInInspector, ReadOnly] private bool isReeling = false; // whether the player is currently pressing the reel button to move the catch slider up
    [ShowInInspector, ReadOnly] private FishState fishState = FishState.Struggling; // Whether the fish is currently up, down, or struggling
    [ShowInInspector,ReadOnly,Range(-1f, 1f)]
    private float caughtProgress = 0f;
    private float timeSinceStateChange = 0f; // How much time has passed since the fish last changed state
    private float currentStateDuration = 0f; // How long the fish will stay in its current state
    private float fishStateStartPosition = 0f; // Fish slider position when current state began
    private float currentMaxMoveDistance = 0f; // Max distance the fish can travel in the current Up/Down state
    private float currentFishVelocity = 0f; // How quickly the fish slider is currently moving, positive is up, negative is down
    private float currentHookVelocity = 0f; // ^ but for the catch slider

    // Variables
    [Header("Catching Settings")]
    public float catchMinSize = 0.1f; // Minimum size of the catch area
    public float catchMaxSize = 0.5f; // Maximum size of the catch area
    public float catchRate = 0.1f; // How quickly the player catches the fish when succesfully catching
    public float escapeRate = 0.2f; // How quickly the fish escapes when the player is not successfully catching
    public float hookAcceleration = 1f; // How quickly the hook accelerates upward when reeling
    public float hookGravity = 0.5f; // Downward acceleration applied when not reeling
    public float hookMaxVelocity = 2f; // Maximum velocity the hook can reach

    [Header("Fish Settings")]
    public float fishMaxVelocity = 1.5f; // Maximum velocity the fish can reach
    public float fishMinMoveDistance = 0.05f; // Minimum distance the fish moves in one direction before struggling
    public float fishMaxMoveDistance = 0.4f; // Maximum distance the fish moves in one direction before struggling
    public float jumpinessMult = 1f; // Multiplier for how twitchy the fish is
    public float speedMult = 1f; // Multiplier for how quickly the fish accelerates
    public float stubbornnessMult = 1f; // Multiplier for how unlikely the fish is to change direction
    public float sizeMult = 1f; // Multiplier for how big the fish is, which will affect catch area
    public float weightMult = 1f; // Multiplier for how weighty the fish is, which will affect changing direction

    public float Jumpiness => fish.jumpiness * jumpinessMult;
    public float Speed => fish.speed * speedMult;
    public float Stubbornness => fish.stubbornness * stubbornnessMult;
    public float Size => fish.size * sizeMult;
    public float Weight => fishWeight * weightMult;
    
    private float fishWeight; // Instance-specific weight value

    // Input actions
    [Header("Input Actions")]
    public InputActionReference reelAction; // button input to reel the hook upward

    // Components
    [Header("Components")]
    public Slider fishSlider; // Reference to the Slider component used for showing the fish location
    public Slider catchSlider; // Reference to the Slider component used for showing the catch progress
    public Image fishImage; // Reference to the Image component of the slider handle
    public Image catchImage; // ^ but for the catch slider
    [HideInInspector] public Fish fish; // Reference to the fish ScriptableObject, set when we create the Stardew instance in the scene

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (fish == null)
        {
            // Grab default fish from Resources if not set in Inspector
            fish = Resources.Load<Fish>("DEFAULT FISH");
        }

        // Initialize instance-specific values
        fishWeight = Random.Range(fish.minSize, fish.maxSize);

        RectTransform hookRect = catchImage.GetComponent<RectTransform>();
        float catchAreaSize = Mathf.Lerp(catchMinSize, catchMaxSize, Size);
        hookRect.sizeDelta = new Vector2(hookRect.sizeDelta.x, catchAreaSize*200f);
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
            currentHookVelocity -= hookGravity * Time.deltaTime;
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
                float deceleration = fishAcceleration / (1f + Weight);
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

        // Derive the catch half-size in normalized slider space from the actual visual hook size
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
            caughtProgress += catchRate * Time.deltaTime;
        }
        else
        {
            caughtProgress -= escapeRate * Time.deltaTime;
        }

        caughtProgress = Mathf.Clamp(caughtProgress, -1f, 1f); // Clamp to either -1 (fully escaped) or 1 (fully caught)

        // Check for win/lose conditions
        if (caughtProgress >= 1f)
        {
            fishState = FishState.Caught;
            Debug.Log("Fish Caught!");
            // TODO: Trigger any catch animations or logic here
        }
        else if (caughtProgress <= -1f)
        {
            fishState = FishState.Escaped;
            Debug.Log("Fish Escaped!");
            // TODO: Trigger any escape animations or logic here
        }
    }
}
