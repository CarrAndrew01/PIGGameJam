using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Class to handle pop-up animations for minigames, etc.
/// </summary>
public class Popup : MonoBehaviour
{
    // State
    [Header("State")]
    [ReadOnly] public bool isPoppedIn = false;
    [ReadOnly] public bool isAnimating = false;
    [ReadOnly] public bool isSwapping = false;
    public bool ReadyForInput => !isAnimating && !isSwapping;

    protected bool isPoppingIn = false;
    protected bool isPoppingOut = false;
    protected bool willClose = false; // If pop-out has been triggered and we are waiting for the animation to finish before actually closing
    protected bool willOpen = false; // ^ same but for pop-in
    protected bool willSwap = false; // ^ same but for swapping menus

    // Variables
    [Header("Settings")]
    public bool destroyCanvasOnPopOut = true; // Whether to destroy the child canvas when popping out
    public float popInDuration = 1f; // Duration of the pop-in animation in seconds
    public float popOutDuration = 1f; // Duration of the pop-out animation in seconds
    public float swapOutDuration = 0.25f; // Duration of the pop-out animation when swapping menus, in seconds
    public float swapInDuration = 0.5f; // Duration of the pop-in animation when swapping menus, in seconds
    [Range(0f, 1f)]
    public float fadeFinishPercent = 0.5f; // Percentage of duration at which fade finishes

    // Components
    [Header("Components")]
    public RectTransform windowRect; // The RectTransform of the popup window, used for scaling animations
    [ShowInInspector, ReadOnly]public Canvas childCanvas;

    protected CanvasGroup windowCanvasGroup;
    protected GameObject nextMenuPrefab; // Used when will open or will swap is true


    void Awake()
    {
        // Send to GameManager
        switch (gameObject.tag)
        {
            case "Minigame":
                GameManager.Instance.minigamePopup = this;
                break;
            case "Menu":
                GameManager.Instance.menuPopup = this;
                break;
            default:
                Debug.LogWarning($"Popup with name {gameObject.name} is not being assigned to a variable in GameManager! Make sure to add it to the switch statement in Popup.Awake().");
                break;
        }

        // Get components and setup
        if (windowRect == null)
        {
            Debug.LogError("Popup is missing a child RectTransform component for the window!");
            return;
        }

        windowCanvasGroup = windowRect.GetComponent<CanvasGroup>();
        if (windowCanvasGroup == null)
        {
            windowCanvasGroup = windowRect.gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected virtual void Start()
    {
        // Ensure the popup starts hidden
        windowRect.localScale = new Vector3(0f, 1f, 1f);
        windowCanvasGroup.alpha = 0f;
    }

    protected virtual void Update()
    {
        if (!isAnimating && !isSwapping)
        {
            if (willClose)
            {
                willClose = false;
                StartCoroutine(TriggerPopOut());
            }
            else if (willOpen)
            {
                willOpen = false;
                StartCoroutine(TriggerPopIn(nextMenuPrefab));
            }
            else if (willSwap)
            {
                willSwap = false;
                StartCoroutine(SwapMenu(nextMenuPrefab));
            }
        }
    }

    // Methods
    public IEnumerator TriggerPopIn(GameObject canvasPrefab, float durationOverride = -1f, bool forceSwap = false, System.Action<GameObject> onComplete = null, System.Action<GameObject> onBeforeShow = null)
    {
        // First check if we are currently popping out or swapping, and if so, set a flag to pop in as soon as we are done
        if (isAnimating && (isPoppingOut || isSwapping))
        {
            willOpen = true;
            nextMenuPrefab = canvasPrefab;
            onComplete?.Invoke(null);
            yield break;
        }

        // Check for existing child canvas and instead swap if there is one
        if (childCanvas != null)
        {
            if (forceSwap)
            {
                // Forced swap: perform swap immediately and return the result via onComplete
                yield return StartCoroutine(TriggerSwap(canvasPrefab, onComplete));
                yield break;
            }

            Debug.Log("Menu exists -- swapping menu after.");
            yield return StartCoroutine(TriggerSwap(canvasPrefab, onComplete));
            yield break;
        }

        if (isPoppedIn || isAnimating)
        {
            onComplete?.Invoke(null);
            yield break;
        }

        // Pops in the popup with the given prefab as a child canvas
        isPoppingIn = true;
        isAnimating = true;
        yield return StartCoroutine(ShowPopup(canvasPrefab, durationOverride > 0f ? durationOverride : popInDuration, onBeforeShow));
        onComplete?.Invoke(childCanvas != null ? childCanvas.gameObject : null);
    }

    public IEnumerator TriggerPopOut(float durationOverride = -1f, System.Action<GameObject> onAfter = null, System.Action<GameObject> onBefore = null)
    {
        // If we're currently popping in or swapping, schedule a close for later and return
        if (isAnimating && (isPoppingIn || isSwapping))
        {
            willClose = true;
            onAfter?.Invoke(null);
            yield break;
        }

        if (!isPoppedIn || isAnimating)
        {
            onAfter?.Invoke(null);
            yield break;
        }

        // Prepare to pop out
        isPoppingOut = true;
        isAnimating = true;

        GameObject canvasObj = childCanvas != null ? childCanvas.gameObject : null;
        onBefore?.Invoke(canvasObj);

        yield return StartCoroutine(HidePopup(durationOverride > 0f ? durationOverride : popOutDuration));

        onAfter?.Invoke(canvasObj);
    }

    protected IEnumerator TriggerSwap(GameObject newMenuPrefab, System.Action<GameObject> onSwapComplete = null)
    {
        if (!isPoppedIn || !ReadyForInput)
        {
            onSwapComplete?.Invoke(null);
            yield break;
        }
        isSwapping = true;
        yield return StartCoroutine(SwapMenu(newMenuPrefab));
        onSwapComplete?.Invoke(childCanvas != null ? childCanvas.gameObject : null);
    }

    // Coroutine to swap from the current menu to a new menu of the given type, by first popping out at a fast speed, then popping in the new menu at a fast speed
    protected IEnumerator SwapMenu(GameObject newMenuPrefab)
    {
        // Pop out at a fast speed
        StartCoroutine(TriggerPopOut(swapOutDuration));
        while (isAnimating)
        {
            yield return null; // Wait until the pop-out animation is finished
        }

        // Pop in the new menu at a fast speed
        yield return StartCoroutine(TriggerPopIn(newMenuPrefab));
        isSwapping = false;
    }

    protected void ResetAnchorPoints()
    {
        // Resets the anchor points of the window rect to be centered, so that scaling animations will work correctly
        windowRect.anchorMin = new Vector2(0.5f, 0.5f);
        windowRect.anchorMax = new Vector2(0.5f, 0.5f);
        windowRect.pivot = new Vector2(0.5f, 0.5f);
        windowRect.anchoredPosition = Vector2.zero;
    }

    // Coroutines for showing and hiding the popup
    protected IEnumerator ShowPopup(GameObject canvasPrefab, float duration, System.Action<GameObject> onBeforeShow = null)
    {
        // Instantiate the canvas prefab as a child of the popup canvas
        childCanvas = Instantiate(canvasPrefab, windowRect).GetComponent<Canvas>();

        // Allow caller to position or modify the spawned canvas before animation starts
        onBeforeShow?.Invoke(childCanvas != null ? childCanvas.gameObject : null);

        // Start with the canvas x scale at 0, y and z at 1, and fully transparent
        windowRect.localScale = new Vector3(0f, 1f, 1f);
        windowCanvasGroup.alpha = 0f;

        float fadeDuration = duration * fadeFinishPercent;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time / duration);
            // Animate only the x scale from 0 to 1
            float x = Mathf.Lerp(0f, 1f, Mathf.Sin(progress * Mathf.PI * 0.5f));
            windowRect.localScale = new Vector3(x, 1f, 1f);

            // Fade in alpha, finishing at fadeDuration
            float fadeProgress = Mathf.Clamp01(time / fadeDuration);
            windowCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeProgress);

            yield return null;
        }
        windowRect.localScale = new Vector3(1f, 1f, 1f); // Ensure it's fully stretched at the end
        windowCanvasGroup.alpha = 1f; // Ensure fully visible
        isPoppedIn = true;
        isPoppingIn = false;
        isAnimating = false;
    }
    [ContextMenu("Test Pop In")]
    protected void TestPopIn() => StartCoroutine(ShowPopup(popInDuration));
    protected IEnumerator ShowPopup(float duration)
    {
        float fadeDuration = duration * fadeFinishPercent;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time / duration);
            // Animate only the x scale from 0 to 1
            float x = Mathf.Lerp(0f, 1f, Mathf.Sin(progress * Mathf.PI * 0.5f));
            windowRect.localScale = new Vector3(x, 1f, 1f);

            // Fade in alpha, finishing at fadeDuration
            float fadeProgress = Mathf.Clamp01(time / fadeDuration);
            windowCanvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeProgress);

            yield return null;
        }
        windowRect.localScale = new Vector3(1f, 1f, 1f); // Ensure it's fully stretched at the end
        windowCanvasGroup.alpha = 1f; // Ensure fully visible
        isPoppedIn = true;
        isPoppingIn = false;
        isAnimating = false;
    }

    [ContextMenu("Test Pop Out")]
    protected void TestPopOut() => StartCoroutine(HidePopup(popOutDuration));
    protected IEnumerator HidePopup(float duration)
    {
        float fadeDuration = duration * fadeFinishPercent;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time / duration);
            float x = Mathf.Lerp(1f, 0f, 1f - Mathf.Cos(progress * Mathf.PI * 0.5f));
            windowRect.localScale = new Vector3(x, 1f, 1f);

            // Fade out alpha, finishing at fadeDuration
            float fadeProgress = Mathf.Clamp01(time / fadeDuration);
            windowCanvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);

            yield return null;
        }
        windowRect.localScale = new Vector3(0f, 1f, 1f); // Ensure it's fully shrunk at the end
        windowCanvasGroup.alpha = 0f; // Ensure fully transparent

        if (destroyCanvasOnPopOut && childCanvas != null)
        {
            Destroy(childCanvas.gameObject);
            childCanvas = null;
        }
        isPoppedIn = false;
        isPoppingOut = false;
        isAnimating = false;

        ResetAnchorPoints();
    }
}
