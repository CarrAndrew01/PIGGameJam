using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Class for the bobber which has its own physics and logic for being throw, bobbing, or reeling.
/// </summary>
public class Bobber : MonoBehaviour
{
    // State
    [Header("State")]
    [ShowInInspector, ReadOnly] private bool bobberInRange = false; // Whether the bobber is currently in range to fish
    [ReadOnly] public FishShadow currentFishShadow; // The fish shadow the player is currently trying to catch, if any
    [ShowInInspector, ReadOnly]
    private Vector2 currentVelocity; // Current velocity of the bobber, used for movement
    [ShowInInspector, ReadOnly]
    private bool isInWater = false;
    [ShowInInspector, ReadOnly]
    private bool isReelingIn = false;
    [ShowInInspector, ReadOnly]
    private float timeInWater = 0f; // Tracks how long the bobber has been in water
    private float timeReelingIn = 0f;

    public CaughtFish FishToCatch => currentFishShadow.fishData;
    public bool isMinigameActive = false;

    // Variables
    [Header("Bobber Settings")]
    public Vector2 minThrowVelocity = new Vector2(0.4f, 2f); // Minimum velocity when throwing the bobber
    public Vector2 maxThrowVelocity = new Vector2(1f, 3f); // Maximum velocity when throwing the bobber
    public float throwAngle = 45f; // Angle at which the bobber is thrown (degrees)
    // TODO: Maybe this should be changeable ^^^

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

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Transform waterMaskTransform; // Used to mask the bobber when it goes below water
    public Collider2D fishTrigger;
    public Transform lineAttachPoint;

    private Transform playerShipTransform;

    public void Init(Transform ship, float throwStrength)
    {
        playerShipTransform = ship;
        ThrowBobber(throwStrength);
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
        }
        else
        {
            timeInWater = 0f;
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
        if (other.CompareTag("Fish"))
        {
            Debug.Log("Bobber entered fish trigger");
            bobberInRange = true;
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
        if (other.CompareTag("Fish"))
        {
            Debug.Log("Bobber exited fish trigger");
            bobberInRange = false;
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

        timeReelingIn += Time.fixedDeltaTime;
        // Move towards the player ship, and destroy when close enough
        Vector3 directionToShip = (playerShipTransform.position - transform.position).normalized;
        currentVelocity = directionToShip * (reelInAcceleration * timeReelingIn);

        if (Vector3.Distance(transform.position, playerShipTransform.position) < 0.5f)
        {
            Fishing.HideCastLine();
            Destroy(gameObject);
        }
    }
    private void ThrowBobber(float throwStrength)
    {
        fishTrigger.enabled = false;
        // Lerp between min and max throw velocity based on strength
        float velocityX = Mathf.Lerp(minThrowVelocity.x, maxThrowVelocity.x, throwStrength);
        float velocityY = Mathf.Lerp(minThrowVelocity.y, maxThrowVelocity.y, throwStrength);

        // Convert angle to radians for calculation
        float angleRad = throwAngle * Mathf.Deg2Rad;

        // Calculate the velocity vector based on the angle (I stole this from google)
        currentVelocity = new Vector2(Mathf.Cos(angleRad) * velocityX, Mathf.Sin(angleRad) * velocityY);
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
        spriteRenderer.transform.localPosition = new Vector3(0f, bobbingOffset, 0f);
    }
    private void MoveWaterMask()
    {
        // Keep the water mask at the waterline
        waterMaskTransform.position = new Vector3(waterMaskTransform.position.x, Environment.WaterHeight, waterMaskTransform.position.z);
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
        float newY = Mathf.MoveTowards(transform.position.y, targetY, bouyancyVelocity * Time.fixedDeltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
