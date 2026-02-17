using UnityEngine;
using System.Collections;

/// <summary>
/// Just a script to make the planet spin in orbit.
/// </summary>
public class SpinningPlanet : MonoBehaviour
{
    // State
    public bool isSpinning = true; // Whether the planet is currently spinning or not
    public float SpinMultiplier { get; private set; } = 1f; // Multiplier for rotation speed, can be used to speed up or slow down the spinning
    public float scaleMultiplier = 1f; // Multiplier for scale, can be used to make the planet larger or smaller
    
    private Vector3 baseScale; // The original scale of the planet, used as a reference for scaling
    private Vector3 scaleStart; // The scale at the start of a lerp transition
    private float currentScaleLerpTime = 0f; // Time accumulator for scaling lerp


    // Variables
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f; // In degrees per second
    public float cloudRotationSpeed = 20f; // ^
    public float scaleLerpTime = 5f; // How many seconds it takes to smoothly transition to a new scale

    // Components
    [Header("Components")]
    public Transform cloudLayer; // Reference to the cloud layer transform

    void Awake()
    {
        baseScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Spin();
        Scale();
    }

    // Methods
    public void SetSpinMultiplier(float multiplier, bool smoothTransition = false, float transitionDuration = 1f)
    {
        // With smooth, uses couroutine
        if (smoothTransition)
            StartCoroutine(SmoothSpinTransition(multiplier, transitionDuration));
        // Otherwise, just set it directly
        else SpinMultiplier = multiplier;
    }
    public void IncreaseSpinMultiplier(float amount, bool smoothTransition = false, float transitionDuration = 1f)
    {
        SetSpinMultiplier(SpinMultiplier + amount, smoothTransition, transitionDuration);
    }

    private void Spin()
    {
        if (isSpinning)
        {
            // Main planet rotation
            transform.Rotate(Vector3.forward, rotationSpeed * SpinMultiplier * Time.deltaTime);

            // Cloud layer rotation
            cloudLayer.Rotate(Vector3.forward, cloudRotationSpeed * Time.deltaTime);
        }
    }

    private void Scale()
    {
        Vector3 targetScale = baseScale * scaleMultiplier;

        // Check for difference
        if (transform.localScale != targetScale)
        {
            if (currentScaleLerpTime == 0f)
                scaleStart = transform.localScale; // Capture starting scale

            currentScaleLerpTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(currentScaleLerpTime / scaleLerpTime);
            
            // Smoothly transition to the new scale over scaleLerpTime seconds
            transform.localScale = Vector3.Lerp(scaleStart, targetScale, lerpValue);
        }
        else
        {
            currentScaleLerpTime = 0f; // Reset the lerp time when the scale is at the target value
        }
    }

    // Coroutine to smoothly transition the spin multiplier over a given duration
    private IEnumerator SmoothSpinTransition(float targetMultiplier, float duration)
    {
        float startMultiplier = SpinMultiplier;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            SpinMultiplier = Mathf.Lerp(startMultiplier, targetMultiplier, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        SpinMultiplier = targetMultiplier; // Ensure it ends at the exact target value
    }
}

