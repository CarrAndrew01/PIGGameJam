using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to handle pop-up animations for minigames, etc.
/// </summary>
public class Popup : MonoBehaviour
{
    // State
    public static Popup Instance { get; private set; }
    public bool isPoppedIn = false;

    // Variables
    public bool destroyCanvasOnPopOut = true; // Whether to destroy the child canvas when popping out
    public float popInDuration = 1f; // Duration of the pop-in animation in seconds
    public float popOutDuration = 1f; // Duration of the pop-out animation in seconds
    [Range(0f, 1f)]
    public float fadeFinishPercent = 0.5f; // Percentage of duration at which fade finishes

    // Components
    public RectTransform windowRect; // The RectTransform of the popup window, used for scaling animations
    public Canvas childCanvas;

    private CanvasGroup windowCanvasGroup;


    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Methods
    public static void TriggerPopIn(GameObject canvasPrefab)
    {
        if (Instance.childCanvas != null || Instance.isPoppedIn)
            return;
        // Pops in the popup with the given prefab as a child canvas
        Instance.StartCoroutine(Instance.ShowPopup(canvasPrefab));
    }

    public static void TriggerPopOut()
    {
        if (!Instance.isPoppedIn)
            return;
        // Pops out the popup, which will also destroy the child canvas if destroyCanvasOnPopOut is true
        Instance.StartCoroutine(Instance.HidePopup());
    }

    // Coroutines for showing and hiding the popup
    private IEnumerator ShowPopup(GameObject canvasPrefab)
    {
        // Instantiate the canvas prefab as a child of the popup canvas
        childCanvas = Instantiate(canvasPrefab, windowRect).GetComponent<Canvas>();

        // Start with the canvas x scale at 0, y and z at 1, and fully transparent
        windowRect.localScale = new Vector3(0f, 1f, 1f);
        windowCanvasGroup.alpha = 0f;

        float fadeDuration = popInDuration * fadeFinishPercent;
        float time = 0f;
        while (time < popInDuration)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time / popInDuration);
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
    }

    private IEnumerator HidePopup()
    {
        float fadeDuration = popOutDuration * fadeFinishPercent;
        float time = 0f;
        while (time < popOutDuration)
        {
            time += Time.deltaTime;
            float progress = Mathf.Clamp01(time / popOutDuration);
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
    }
}
