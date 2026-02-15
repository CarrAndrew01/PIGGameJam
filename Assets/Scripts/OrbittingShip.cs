using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Just handles animations for the orbitting ship.
/// </summary>

public class OrbittingShip : MonoBehaviour
{
    // State
    public bool isOrbitting = true; // Whether the ship is currently orbitting or not
    public bool IsApproaching => !isOrbitting; // Whether the ship is currently approaching the planet (not orbitting)
    public bool hasLanded = false; // Whether the ship has landed on the planet or not (end state after approaching)

    private Vector2 orbitBasePosition;
    private float bobbingTime;
    private Vector2 approachStartPosition;
    private float approachTime;

    // Variables
    [Header("Bobbing Settings")]
    public float bobbingSpeed = 1f;
    public float bobbingAmplitude = 1f; // How far the ship bobs up and down

    [Header("Approach Settings")]
    public AnimationCurve approachCurve; // Curve for the vertical path when approaching 
    public float approachHeight = 5f; // Multiplier for curve value
    public float approachDuration = 3f; // How many seconds the approach takes
    public float planetScaleAtLanding = 3f;
    public float shipScaleAtLanding = 0.25f;

    [Header("Input Actions")]
    public InputActionReference landAction; // Expects Button, used to trigger the approach when pressed

    // Components
    [Header("Components")]
    public Transform landingPoint; // Point on the planet where the ship should land when it stops approaching
    public SpinningPlanet planet; // Reference to the planet script, used to trigger the expanding

    void Awake()
    {
        orbitBasePosition = transform.position;

        // Set a nice default approach curve if none was assigned in the Inspector
        if (approachCurve == null || approachCurve.length == 0)
        {
            approachCurve = new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 2f),    // start on the line, ease out upward
                new Keyframe(0.35f, 0.6f, 0f, 0f), // peak offset at ~35% progress
                new Keyframe(0.7f, 0.15f, -0.5f, -0.5f), // settling back down
                new Keyframe(1f, 0f, -0.3f, 0f)  // end exactly on the landing point
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasLanded) return;

        if (isOrbitting)
        {
            if (landAction.action.WasPressedThisFrame())
                BeginApproach();
            else
                Bob();
        }
        else Approach();
    }

    public void BeginApproach()
    {
        if (!isOrbitting || hasLanded) return;

        isOrbitting = false;
        approachStartPosition = transform.position;
        approachTime = 0f;

        // Trigger planet expansion (to look like the ship is getting closer)
        planet.SetSpinMultiplier(0f, smoothTransition: true, transitionDuration: 2f);
        planet.scaleMultiplier = planetScaleAtLanding;
    }

    private void Bob()
    {
        bobbingTime += Time.deltaTime * bobbingSpeed;

        float yOffset = Mathf.Sin(bobbingTime * Mathf.PI * 2f) * bobbingAmplitude;

        transform.position = orbitBasePosition + Vector2.up * yOffset;
    }

    private void Approach()
    {
        approachTime += Time.deltaTime;
        float t = Mathf.Clamp01(approachTime / approachDuration);

        // X & Y: linear straight-line path from start to landing
        float x = Mathf.Lerp(approachStartPosition.x, landingPoint.position.x, t);
        float y = Mathf.Lerp(approachStartPosition.y, landingPoint.position.y, t);

        // Add curve-driven vertical offset (curve value × approachHeight)
        y += approachCurve.Evaluate(t) * approachHeight;

        transform.position = new Vector2(x, y);

        // Landing check
        if (t >= 1f)
        {
            transform.position = landingPoint.position;
            hasLanded = true;
            StartCoroutine(SmoothShrink(shipScaleAtLanding, 2f));
        }
    }

    // Coroutine for smoothly shrinking the ship's scale
    private IEnumerator SmoothShrink(float targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.one * targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one * targetScale;

        // TODO: proper next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1); // For now, just load the next scene

    }
}
