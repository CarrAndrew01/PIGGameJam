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

    // Variables
    public float popInDuration = 1f; // Duration of the pop-in animation in seconds
    public float popOutDuration = 1f; // Duration of the pop-out animation in seconds
    [Range(0f, 1f)]
    public float fadeFinishPercent = 0.5f; // Percentage of duration at which fade finishes

    // Components
    public RectTransform windowRect; // The RectTransform of the popup window, used for scaling animations
    public Canvas minigameCanvas;

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
    public static void TriggerPopIn(GameObject minigamePrefab)
    {
        if (Instance.minigameCanvas != null)
            return;

        Instance.StartCoroutine(Instance.ShowPopup(minigamePrefab));
    }

    public static void TriggerPopOut()
    {
        Instance.StartCoroutine(Instance.HidePopup());
    }

    // Coroutines for showing and hiding the popup
    private IEnumerator ShowPopup(GameObject minigamePrefab)
    {
        // Instantiate the minigame prefab as a child of the popup canvas
        minigameCanvas = Instantiate(minigamePrefab, windowRect).GetComponent<Canvas>();

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

        Destroy(minigameCanvas.gameObject); // Destroy the minigame but leave the popup canvas
        minigameCanvas = null;
    }
}
