using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;

/// <summary>
/// Class for the fish moving in the planet scene. They will simply move around a little from their spawn and have a trigger to begin minigames.
/// </summary>
public class FishShadow : MonoBehaviour
{
    // State
    [Header("State")]
    public CaughtFish fishData; // The data for the fish this shadow represents
    [ReadOnly] public Vector2 initialPosition; // The position the fish spawned at, which it will move around
    [ReadOnly] public Vector2 movementDirection; // The direction the fish is currently moving in
    [ReadOnly] public int failCount = 0; // How many times the player has failed to catch this fish so far, which will be used to determine if the fish should escape
    [ReadOnly] public bool IsEscaping { get; private set; } = false; // Whether the fish is currently escaping, which will trigger it to stop moving and start shrinking until it disappears
    [ReadOnly] public bool pauseTimer = false; // Whether the leave timer should be paused
    [ShowInInspector, ReadOnly] private float leaveTimer = 0f; // Timer to track how long the fish has been present
    [ShowInInspector, ReadOnly] private Vector2 targetDirection;
    [ShowInInspector, ReadOnly] private float directionChangeTimer = 0f;
    public bool IsHooked { get; private set; } = false; // Whether the fish is currently hooked by the bobber
    [ReadOnly] public Bobber targetBobber;

    // Variables
    [Header("Settings")]
    public int numberOfFailsBeforeEscape = 2; // How many times the player can fail to catch this fish before it escapes
    public float timeUntilLeaving = 20f; // How long the fish will stay before it leaves on its own, in seconds

    [Header("Movement")]
    public float movementRadius = 2f; // How far from its initial position the fish will move
    public float movementSpeed = 1f; // How fast the fish moves
    public float verticalMovementMax = 0.2f; // The maximum vertical movement for the fish
    public float investigateBaitSpeed = 1.4f;

    [Header("Vertical Limits")]
    public float minHeight = float.NegativeInfinity; // Minimum world Y for the sprite
    public float maxHeight = float.PositiveInfinity; // Maximum world Y for the sprite

    [Header("Hooked Movement")]
    public float hookedRadiusMult = 1.5f; // Multiplies movementRadius when hooked
    public float hookedMovementSpeed = 4f;
    public float hookedDirectionMult = 2f; // Multiplies the randomness of the direction changes when hooked to make it more erratic

    [Header("Shrink/Grow")]
    public float fishShrinkDuration = 0.5f;
    public float fishGrowDuration = 2f;

    [Header("Direction Change")]
    public float directionChangeInterval = 2f; // seconds between direction changes
    public float directionLerpSpeed = 2f; // speed of interpolation

    [Header("Fish Preview")]
    public Vector3 previewOffset = new Vector3(0f, 0.5f, 0f); // Offset for the fish preview from the fish shadow's position
    public float previewBobAmplitude = 0.1f; // Amplitude of the bobbing motion for the fish preview
    public float previewBobFrequency = 1f; // Frequency of the bobbing motion for

    private float statFishWeight;
    private int amountInCatch = 1; // determines if extra fish are added if we successfully catch

    // Components
    [Header("Components")]
    public Transform previewTransform;
    public Transform spriteTransform;

    private Transform shipTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Grab stats
        statFishWeight = GameManager.GetPlayerStat(StatType.fishWeight);

        // Get the initial position
        initialPosition = transform.position;
        shipTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Generate fish data for this shadow.
        Fish fish = Environment.GetRandomFish();
        fishData = new CaughtFish
        {
            fish = fish,
            weight = Random.Range(fish.minWeight, fish.maxWeight) * statFishWeight,
            planetOfOrigin = Environment.CurrentEnvironment.name
        };

        amountInCatch = Random.Range(fish.minAmount, fish.maxAmount + 1); // +1 because Random.Range is exclusive of the upper bound

        // Set the preview sprite to match the fish type
        if (previewTransform != null && fish.sprite != null)
        {
            SpriteRenderer sr = previewTransform.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = fish.sprite;

                // Only enable if the player has the stat
                if (GameManager.GetPlayerStat(StatType.fishPreview) > 0)
                    previewTransform.gameObject.SetActive(true);
                else
                    previewTransform.gameObject.SetActive(false);
            }
        }

        // Start the grow animation
        StartCoroutine(GrowFromZero());

        // Set the initial direction
        targetDirection = movementDirection = Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEscaping) return; // If the fish is escaping, don't bother updating movement

        BobPreview();
        HandleDirectionChange();
        Swim();

        // Count up the leave timer and check if the fish should leave on its own
        // If theres a target bobber, we don't want the fish to leave on its own
        if (!pauseTimer && targetBobber == null)
        {
            leaveTimer += Time.deltaTime;
            if (leaveTimer > timeUntilLeaving)
            {
                Escape();
            }
        }
    }

    // Methods
    public void BeginFishing()
    {
        pauseTimer = true; // Pause the leave timer while the player is trying to catch the fish
        IsHooked = true;
        targetBobber.isMinigameActive = true;

        Fishing.StartFishingMinigame(this);
    }
    public void EndFishing(bool caught)
    {
        pauseTimer = false; // Unpause the leave timer when the player is done trying to catch the fish
        IsHooked = false;
        if (targetBobber != null)
        {
            targetBobber.isMinigameActive = false;
            targetBobber = null;
        }

        if (caught)
        {
            Catch();
        }
        else
        {
            AddFail();
        }

        Fishing.EndFishingMinigame();
    }
    public void ResetLeaveTimer()
    {
        leaveTimer = 0f;
    }
    public void Catch()
    {
        // Trigger catch logic, such as playing an animation or sound effect
        Debug.Log("Fish Caught!");

        // Add the primary fish to the player's inventory
        GameManager.AddFishToInventory(fishData);

        // Add any additional fish if amountInCatch > 1
        for (int i = 1; i < amountInCatch; i++)
        {
            // Create additional caught fish for any extra amount
            CaughtFish extraFish = new CaughtFish()
            {
                fish = fishData.fish,
                weight = Random.Range(fishData.fish.minWeight, fishData.fish.maxWeight) * statFishWeight,
                planetOfOrigin = Environment.CurrentEnvironment != null ? Environment.CurrentEnvironment.name : fishData.planetOfOrigin
            };
            GameManager.AddFishToInventory(extraFish);
        }


        // Destroy the fish shadow since it's been caught
        Destroy(gameObject);
    }
    public void AddFail()
    {
        failCount++;
        if (failCount >= numberOfFailsBeforeEscape)
        {
            Escape();
        }
    }
    public void Escape()
    {
        // Trigger escape logic, such as playing an animation or sound effect
        Debug.Log("Fish Escaped!");

        // Start the shrink and destroy coroutine
        IsEscaping = true;
        StartCoroutine(ShrinkAndDestroy());
    }

    private void Swim()
    {
        // Smoothly interpolate movementDirection toward targetDirection
        movementDirection = Vector2.Lerp(movementDirection, targetDirection, directionLerpSpeed * Time.deltaTime);

        float currentMovementSpeed = movementSpeed;
        if (IsHooked)
            currentMovementSpeed = hookedMovementSpeed;
        else if (targetBobber != null)
            currentMovementSpeed = investigateBaitSpeed;
        Vector3 movementVector = (Vector3)movementDirection * currentMovementSpeed * Time.deltaTime;

        // Move fish horizontally
        transform.position += new Vector3(movementVector.x, 0f, 0f);
        // Move sprite vertically and clamp to min/max world heights
        float newLocalY = spriteTransform.localPosition.y + movementVector.y;
        float worldY = transform.position.y + newLocalY;
        float clampedWorldY = Mathf.Clamp(worldY, minHeight, maxHeight);
        float clampedLocalY = clampedWorldY - transform.position.y;
        spriteTransform.localPosition = new Vector3(0f, clampedLocalY, 0f);
    }
    private void BobPreview()
    {
        if (previewTransform != null && previewTransform.gameObject.activeSelf) // Only bob if the preview is active
        {
            float bobbingOffset = Mathf.Sin(Time.time * previewBobFrequency) * previewBobAmplitude;
            previewTransform.localPosition = previewOffset + new Vector3(0f, bobbingOffset, 0f);
        }
    }
    private void HandleDirectionChange()
    {
        // Shift the initial direction based on the position of the player ship when hooked, as well as decreasing the radius based on Fishing.reelInFactor
        Vector2 radiusPoint = initialPosition;
        float currentRadius = movementRadius;
        if (IsHooked && shipTransform != null)
        {
            float reelInFactor = Fishing.reelInFactor;
            radiusPoint = Vector2.Lerp(initialPosition, (Vector2)shipTransform.position, reelInFactor);
            currentRadius = Mathf.Lerp(movementRadius * hookedRadiusMult, movementRadius * 0.5f, reelInFactor);
        }
        float currentDirectionChangeInterval = IsHooked ? directionChangeInterval / hookedDirectionMult : directionChangeInterval;

        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= currentDirectionChangeInterval)
        {
            directionChangeTimer = 0f;
            // If there is a target bobber, go towards that instead of a random direction
            if (targetBobber != null && !targetBobber.isMinigameActive)
            {
                targetDirection = ((Vector2)targetBobber.transform.position - (Vector2)spriteTransform.position).normalized;

                // Check if in hook range (both the transform and the sprite, since sprite is vertically offset)
                if (Vector2.Distance(targetBobber.transform.position, transform.position) < targetBobber.hookRange
                    || Vector2.Distance(targetBobber.spriteRenderer.transform.position, spriteTransform.position) < targetBobber.hookRange)
                {
                    // If the fish is in hook range, hook it (if it likes the bait)
                    if (!IsHooked && (fishData.fish.preferredBaitType == 0 || fishData.fish.preferredBaitType == GameManager.GetPlayerStat(StatType.baitType)))
                    {
                        IsHooked = true;
                        BeginFishing();
                    }
                    else
                    {
                        targetBobber = null;
                    }
                }
                return;
            }
            else
                targetBobber = null;

            // If either the object or its sprite are outside movement radius, head back toward initial position
            if (Vector2.Distance(transform.position, radiusPoint) > currentRadius
                || (spriteTransform != null && Vector2.Distance(spriteTransform.position, radiusPoint) > currentRadius))
            {
                targetDirection = (radiusPoint - (Vector2)transform.position).normalized;
            }
            else
            {
                targetDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-verticalMovementMax, verticalMovementMax)).normalized;
            }
        }
    }

#if UNITY_EDITOR
    // Draw movement radius when selected in the editor
    void OnDrawGizmosSelected()
    {
        // Use initialPosition when playing, otherwise use current transform position
        Vector3 center = Application.isPlaying ? new Vector3(initialPosition.x, initialPosition.y, transform.position.z) : transform.position;

        // Base radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, movementRadius);

        // Show hooked radius if different
        if (hookedRadiusMult != 1f)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(center, movementRadius * hookedRadiusMult);
        }

        // Draw min/max horizontal lines if finite
        float currentRadius = movementRadius * (hookedRadiusMult != 0f ? hookedRadiusMult : 1f);
        if (!float.IsInfinity(minHeight))
        {
            Vector3 left = new Vector3(center.x - currentRadius, minHeight, center.z);
            Vector3 right = new Vector3(center.x + currentRadius, minHeight, center.z);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(left, right);
        }
        if (!float.IsInfinity(maxHeight))
        {
            Vector3 left = new Vector3(center.x - currentRadius, maxHeight, center.z);
            Vector3 right = new Vector3(center.x + currentRadius, maxHeight, center.z);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(left, right);
        }
    }
#endif

    // Coroutine to shrink the fish shadow before destroying it
    private IEnumerator ShrinkAndDestroy()
    {
        Vector3 originalScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < fishShrinkDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / fishShrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    // Coroutine to grow the fish shadow from zero to its original scale when it spawns
    private IEnumerator GrowFromZero()
    {
        Vector3 targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < fishGrowDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / fishGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure it ends at the exact target scale
    }
}
