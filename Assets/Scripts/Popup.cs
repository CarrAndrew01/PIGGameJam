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
    public Canvas childCanvas;

    private CanvasGroup windowCanvasGroup;


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

    void Start()
    {
        // Ensure the popup starts hidden
        windowRect.localScale = new Vector3(0f, 1f, 1f);
        windowCanvasGroup.alpha = 0f;
    }

    // Methods
    public GameObject TriggerPopIn(GameObject canvasPrefab, float durationOverride = -1f)
    {
        if (childCanvas != null || isPoppedIn || isAnimating)
            return null;
        // Pops in the popup with the given prefab as a child canvas
        isAnimating = true;
        StartCoroutine(ShowPopup(canvasPrefab, durationOverride > 0f ? durationOverride : popInDuration));
        return childCanvas.gameObject;
    }

    public void TriggerPopOut(float durationOverride = -1f)
    {
        if (!isPoppedIn || isAnimating)
            return;
        // Pops out the popup, which will also destroy the child canvas if destroyCanvasOnPopOut is true
        isAnimating = true;
        StartCoroutine(HidePopup(durationOverride > 0f ? durationOverride : popOutDuration));
    }

    public IEnumerator TriggerSwap(GameObject newMenuPrefab, System.Action<GameObject> onSwapComplete = null)
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
    private IEnumerator SwapMenu(GameObject newMenuPrefab)
    {
        // Pop out at a fast speed
        TriggerPopOut(swapOutDuration);
        while (isAnimating)
        {
            yield return null; // Wait until the pop-out animation is finished
        }

        // Pop in the new menu at a fast speed
        TriggerPopIn(newMenuPrefab);
        while (isAnimating)
        {
            yield return null; // Wait until the pop-in animation is finished
        }
        isSwapping = false;
    }

    // Coroutines for showing and hiding the popup
    private IEnumerator ShowPopup(GameObject canvasPrefab, float duration)
    {
        // Instantiate the canvas prefab as a child of the popup canvas
        childCanvas = Instantiate(canvasPrefab, windowRect).GetComponent<Canvas>();

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
        isAnimating = false;
    }
    [ContextMenu("Test Pop In")]
    private void TestPopIn() => StartCoroutine(ShowPopup(popInDuration));
    private IEnumerator ShowPopup(float duration)
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
        isAnimating = false;
    }

    [ContextMenu("Test Pop Out")]
    private void TestPopOut() => StartCoroutine(HidePopup(popOutDuration));
    private IEnumerator HidePopup(float duration)
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
        isAnimating = false;
    }
}
