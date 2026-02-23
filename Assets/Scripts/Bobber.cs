using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Class for the bobber which has its own physics and logic for being throw, bobbing, or reeling.
/// </summary>
public class Bobber : MonoBehaviour
{
    // State
    [Header("State")]
    [ReadOnly] public FishShadow currentFishShadow; // The fish shadow the player is currently trying to catch, if any
    [ShowInInspector, ReadOnly]
    private Vector2 currentVelocity; // Current velocity of the bobber, used for movement
    [ShowInInspector, ReadOnly]
    private bool isInWater = false;
    [ShowInInspector, ReadOnly]
    private bool isReelingIn = false;
    [ShowInInspector, ReadOnly]
    private bool isHooked = false;
    private float timeInWater = 0f; // Tracks how long the bobber has been in water
    private float timeReelingIn = 0f;
    private float currentWaterHeight = 0f; // The current height of the water, used for moving the water mask with fish shadows
    private float currentSpriteOffset = 0f;
    private Vector2 cachedShadowSpritePosition;

    public bool isMinigameActive = false;

    // Variables
    [Header("Bobber Settings")]
    public Vector2 minThrowVelocity = new Vector2(0.4f, 2f); // Minimum velocity when throwing the bobber
    public Vector2 maxThrowVelocity = new Vector2(1f, 3f); // Maximum velocity when throwing the bobber
    public float throwAngle = 45f; // Angle at which the bobber is thrown (degrees)
    // TODO: Maybe this should be changeable ^^^
    public float hookedVelocity = 1f; // How fast the bobber moves towards the fish shadow when a fish is hooked
    public float distanceToRetrieve = 0.5f; // How close the bobber needs to be to the player to be retrieved when reeling in
    public float hookedOffset = -0.5f; // How far from the fish's shadow the bobber should be hooked

    [Header("Movement Settings")]
    public float drag = 0.1f; // How much the bobber slows down over time when in the air
    public float gravity = 9.8f; // How much the bobber is pulled downwards when in the air
    public float waterResistance = 0.5f; // How much the bobber is slowed when in water
    public float bouyancyVelocity = 0.5f;
    public float reelInAcceleration = 1f;

    [Header("Bobbing Settings")]
    public float bobBeginWait = 0.5f; // How long the bobber waits after hitting the water before it starts bobbing
    public float bobbingAmplitude = 0.5f; // How high the bobber bobs up and down
    public float bobbingFrequency = 1f; // How fast the bobber bobs up and down
    public float bobbingResetSpeed = 0.5f; // How fast the bobber resets to the normal position when not bobbing

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Transform waterMaskTransform; // Used to mask the bobber when it goes below water
    public Collider2D fishTrigger;
    public Transform lineAttachPoint;

    private Transform playerShipTransform;

    void Start()
    {
        currentWaterHeight = Environment.WaterHeight;
    }

    public void Init(Transform ship, float throwStrength, float direction = 1f)
    {
        playerShipTransform = ship;
        ThrowBobber(throwStrength, direction);
    }

    void Update()
    {
        // Bobbing is applied to sprite, therefore not in fixed update
        if (isInWater && !isReelingIn)
        {
            if (timeInWater < bobBeginWait)
            {
                timeInWater += Time.deltaTime;
                if (timeInWater > bobBeginWait)
                    timeInWater = bobBeginWait;
            }
            ApplyBobbing();
            CheckHooked();
        }
        else
        {
            timeInWater = 0f;
            ResetBobbing();
        }
    }

    void FixedUpdate()
    {
        // Physics is in fixed update
        if (isReelingIn)
        {
            ApplyVelocity();
            ReelInBobber();
            MoveWaterMask();
        }
        else if (!isInWater)
        {
            ApplyVelocity();
            ApplyDrag();
            ApplyGravity();
            CheckIfInWater();
            MoveWaterMask();
        }
        else
        {
            // Pulls the bobber directly toward the waterline when in water
            ApplyBuoyancy();
            MoveWaterMask();
            if (!currentVelocity.Equals(Vector2.zero))
            {
                ApplyWaterResistance();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish") && Fishing.LastFishShadow == null)
        {
            Debug.Log("Bobber entered fish trigger");
            currentFishShadow = other.GetComponent<FishShadow>();

            if (!currentFishShadow.IsEscaping && !isMinigameActive)
            {
                isMinigameActive = true;
                Fishing.StartFishingMinigame();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fish") && other.GetComponent<FishShadow>() == currentFishShadow)
        {
            Debug.Log("Bobber exited fish trigger");
            currentFishShadow = null;
        }
    }

    // Methods
    public void BeginReelIn()
    {
        isReelingIn = true;
    }

    private void ReelInBobber()
    {
        if (fishTrigger.enabled)
            fishTrigger.enabled = false;

        isHooked = false; // Unhook the bobber when reeling in, so it doesn't get stuck on the water surface or something
        timeReelingIn += Time.fixedDeltaTime;
        // Move towards the player ship, and destroy when close enough
        Vector3 directionToShip = (playerShipTransform.position - transform.position).normalized;
        currentVelocity = directionToShip * (reelInAcceleration * timeReelingIn);

        if (Vector3.Distance(transform.position, playerShipTransform.position) < distanceToRetrieve)
        {
            Fishing.HideCastLine();
            Destroy(gameObject);
        }
        else if (timeReelingIn > 5f) // Just in case something goes wrong and we don't get close enough to the ship, we don't want the bobber to fly around forever
        {
            Fishing.HideCastLine();
            Destroy(gameObject);
        }
    }
    private void ThrowBobber(float throwStrength, float direction = 1f)
    {
        fishTrigger.enabled = false;
        // Lerp between min and max throw velocity based on strength
        float velocityX = Mathf.Lerp(minThrowVelocity.x, maxThrowVelocity.x, throwStrength);
        float velocityY = Mathf.Lerp(minThrowVelocity.y, maxThrowVelocity.y, throwStrength);

        // Convert angle to radians for calculation
        float angleRad = throwAngle * Mathf.Deg2Rad;

        // Calculate the velocity vector based on the angle (I stole this from google)
        currentVelocity = new Vector2(Mathf.Cos(angleRad) * velocityX * direction, Mathf.Sin(angleRad) * velocityY);
    }
    private void CheckHooked()
    {
        bool shouldBeHooked = Fishing.LastFishShadow != null && Fishing.LastFishShadow.IsHooked;
        if (isHooked != shouldBeHooked)
        {
            if (shouldBeHooked)
            {
                isHooked = true;
                // Snap bobber to water height
                transform.position = new Vector3(transform.position.x, Environment.WaterHeight, transform.position.z);
            }
            else
            {
                isHooked = false;
            }
        }
    }
    private void CheckIfInWater()
    {
        // Check if the bobber is below the water height of the current environment
        bool wasInWater = isInWater;
        isInWater = transform.position.y <= Environment.WaterHeight;
        // Reset timer if just entered water
        if (isInWater && !wasInWater)
        {
            timeInWater = 0f;
            fishTrigger.enabled = true; // Enable the fish trigger when we hit the water so we can start catching fish
        }
    }
    private void ApplyBobbing()
    {
        // Lerp amplitude in over bobBeginWait seconds
        float lerpTime = Mathf.Clamp01(timeInWater / bobBeginWait);
        float currentAmplitude = Mathf.Lerp(0f, bobbingAmplitude, lerpTime);
        float bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * currentAmplitude;
        spriteRenderer.transform.localPosition = new Vector3(0f, bobbingOffset + currentSpriteOffset, 0f);
    }
    private void ResetBobbing()
    {
        // Simply begin lerping the sprite back to the usual position so it reeling in looks better
        spriteRenderer.transform.localPosition = Vector3.Lerp(spriteRenderer.transform.localPosition, Vector3.zero, Time.deltaTime * bobbingResetSpeed);

        // Also lerp the current sprite and water heights back to the usual
        currentSpriteOffset = Mathf.Lerp(currentSpriteOffset, 0f, Time.deltaTime * bobbingResetSpeed);
        currentWaterHeight = Mathf.Lerp(currentWaterHeight, Environment.WaterHeight, Time.deltaTime * bobbingResetSpeed);

        if (Vector3.Distance(spriteRenderer.transform.localPosition, Vector3.zero) < 0.01f)
        {
            spriteRenderer.transform.localPosition = Vector3.zero;
            currentSpriteOffset = 0f;
            currentWaterHeight = Environment.WaterHeight;
        }
    }
    private void MoveWaterMask()
    {
        // Keep the water mask at the waterline
        if (isHooked && Fishing.LastFishShadow != null)
        {
            // If a fish is hooked, we want the water mask to follow the height of the fish shadow's sprite instead
            currentWaterHeight = Fishing.LastFishShadow.spriteTransform.position.y;
        }

        waterMaskTransform.position = new Vector3(waterMaskTransform.position.x, currentWaterHeight, waterMaskTransform.position.z);
    }
    private void ApplyVelocity()
    {
        transform.position += (Vector3)currentVelocity * Time.fixedDeltaTime;
    }
    private void ApplyDrag()
    {
        currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, drag * Time.fixedDeltaTime);
    }
    private void ApplyGravity()
    {
        currentVelocity += Vector2.down * gravity * Time.fixedDeltaTime;
    }
    private void ApplyWaterResistance()
    {
        // Unlike drag, we lerp directly to zero to simulate quickly coming to a stop
        currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, waterResistance * Time.fixedDeltaTime);
    }
    private void ApplyBuoyancy()
    {
        float targetY = Environment.WaterHeight;
        float newX = transform.position.x, newY = transform.position.y;

        if (isHooked && Fishing.LastFishShadow != null)
        {
            // If a fish is hooked, we want the bobber to be pulled towards the height of the fish shadow's sprite instead
            cachedShadowSpritePosition = Fishing.LastFishShadow.spriteTransform.localPosition;
            targetY = cachedShadowSpritePosition.y + hookedOffset;

            // Move with hookedVelocity if hooked to simulate being pulled by the fish
            newY = Mathf.MoveTowards(currentSpriteOffset, targetY, hookedVelocity * Time.fixedDeltaTime);

            // Also move the whole bobber left or right with the fish shadow parent to simulate being pulled by the fish
            newX = Mathf.MoveTowards(transform.position.x, Fishing.LastFishShadow.transform.position.x, hookedVelocity * Time.fixedDeltaTime);

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            // Apply vertical offset to the sprite, making it follow the fish (visually)
            currentSpriteOffset = newY;
        }
        else
        {
            // Usual bouyancy when not hooked
            newY = Mathf.MoveTowards(transform.position.y, targetY, bouyancyVelocity * Time.fixedDeltaTime);

            // Apply real vertical movement, to make the bobber go to actually touch the water and not just visually
            transform.position = new Vector3(newX, newY, transform.position.z);

            // We still want vertical movement of the sprite, but only to the last fish's sprite location and without hooked offset
            currentSpriteOffset = Mathf.MoveTowards(currentSpriteOffset, cachedShadowSpritePosition.y, bouyancyVelocity * Time.fixedDeltaTime);
        }
    }
}
