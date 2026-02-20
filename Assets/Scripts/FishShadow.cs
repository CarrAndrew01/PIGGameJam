using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;

/// <summary>
/// Class for the fish moving in the planet scene. They will simply move around a little from their spawn and have a trigger to begin minigames.
/// </summary>
public class FishShadow : MonoBehaviour
{
    // State
    [Header("State")]
    [ReadOnly] public CaughtFish fishData; // The data for the fish this shadow represents
    [ReadOnly] public Vector2 initialPosition; // The position the fish spawned at, which it will move around
    [ReadOnly] public Vector2 movementDirection; // The direction the fish is currently moving in
    [ReadOnly] public int failCount = 0; // How many times the player has failed to catch this fish so far, which will be used to determine if the fish should escape
    [ReadOnly] public bool IsEscaping { get; private set; } = false; // Whether the fish is currently escaping, which will trigger it to stop moving and start shrinking until it disappears
    [ReadOnly] public bool pauseTimer = false; // Whether the leave timer should be paused
    [ShowInInspector, ReadOnly] private float leaveTimer = 0f; // Timer to track how long the fish has been present, which will be used to determine if it should leave on its own

    // Variables
    [Header("Settings")]
    public int numberOfFailsBeforeEscape = 2; // How many times the player can fail to catch this fish before it escapes
    public float timeUntilLeaving = 20f; // How long the fish will stay before it leaves on its own, in seconds

    [Header("Movement")]
    public float movementRadius = 2f; // How far from its initial position the fish will move
    public float movementSpeed = 1f; // How fast the fish moves
    public float verticalMovementMax = 0.2f; // The maximum vertical movement for the fish, which will make their movement less linear and more natural

    [Header("Fish Preview")]
    public Vector3 previewOffset = new Vector3(0f, 0.5f, 0f); // Offset for the fish preview from the fish shadow's position
    public float previewBobAmplitude = 0.1f; // Amplitude of the bobbing motion for the fish preview
    public float previewBobFrequency = 1f; // Frequency of the bobbing motion for

    // Components
    [Header("Components")]
    public Transform previewTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the initial position
        initialPosition = transform.position;

        // Generate fish data for this shadow.
        Fish fish = Environment.GetRandomFish();
        fishData = new CaughtFish
        {
            fish = fish,
            weight = Random.Range(fish.minWeight, fish.maxWeight),
            planetOfOrigin = Environment.CurrentEnvironment.name
        };

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
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEscaping) return; // If the fish is escaping, don't bother updating movement

        BobPreview();
        DetermineMovementDirection();
        transform.position += (Vector3)movementDirection * movementSpeed * Time.deltaTime;

        // Count up the leave timer and check if the fish should leave on its own
        if (!pauseTimer)
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
    }
    public void EndFishing()
    {
        pauseTimer = false; // Unpause the leave timer when the player is done trying to catch the fish
    }
    public void ResetLeaveTimer()
    {
        leaveTimer = 0f;
    }
    public void Catch()
    {
        // Trigger catch logic, such as playing an animation or sound effect
        Debug.Log("Fish Caught!");

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

    private void BobPreview()
    {
        if (previewTransform != null && previewTransform.gameObject.activeSelf) // Only bob if the preview is active
        {
            float bobbingOffset = Mathf.Sin(Time.time * previewBobFrequency) * previewBobAmplitude;
            previewTransform.localPosition = previewOffset + new Vector3(0f, bobbingOffset, 0f);
        }
    }
    private void DetermineMovementDirection()
    {
        // Randomly change direction at intervals, and also ensure the fish generally stays within its movement radius
        if (Vector2.Distance(transform.position, initialPosition) > movementRadius)
        {
            // If we're outside the movement radius, head back towards the initial position
            movementDirection = (initialPosition - (Vector2)transform.position).normalized;
        }
        else if (Random.value < 0.01f) // Random chance to change direction
        {
            movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-verticalMovementMax, verticalMovementMax)).normalized;
        }
    }

    // Coroutine to shrink the fish shadow before destroying it
    private IEnumerator ShrinkAndDestroy()
    {
        float shrinkDuration = 0.5f;
        Vector3 originalScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
