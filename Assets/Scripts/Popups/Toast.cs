using System.Collections;
using Generic = System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public struct ToastData
{
    public string message;
    public string substring1;
    public string substring2;
    public Sprite icon;
    public float duration;

    public ToastData(string message, string substring1, string substring2, Sprite icon, float duration)
    {
        this.message = message;
        this.substring1 = substring1;
        this.substring2 = substring2;
        this.icon = icon;
        this.duration = duration;
    }
}

/// <summary>
/// Class for a simple toast popup, which just shows a message for a short time and then disappears,
/// without blocking input to the rest of the game. 
/// </summary>
public class Toast : Popup
{
    public static Toast Instance { get; private set; }

    [Header("Toast State")]
    public Generic.Queue<ToastData> toastStack = new Generic.Queue<ToastData>();

    public float LastToastTime { get; private set; } = 0f;
    [Header("Toast Settings")]
    public float defaultDuration = 2f;
    public float multPerCharacter = 0.1f; // Multiplier for the duration based on the length of the message, so longer messages stay on screen longer

    [Header("Toast Prefabs")]
    public GameObject toastPrefab;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of Toast detected! Destroying duplicate.");
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

    protected override void Update()
    {
        base.Update();

        // If there are no toasts in the stack, do nothing
        if (toastStack.Count == 0)
            return;

        // If the current toast has finished popping out
        if (!isAnimating && childCanvas == null)
        {
            // If there are more toasts in the stack, show the next one
            if (toastStack.Count > 0)
            {
                StartCoroutine(TriggerPopIn(toastPrefab));
            }
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Static methods
    public static void ShowToast(string message, string substring1 = "", string substring2 = "", Sprite icon = null)
    {
        if (Instance == null)
        {
            Debug.LogError("No instance of Toast found in the scene! Cannot show toast.");
            return;
        }

        // NOTE: I know this is a crazy hack to get this working -- I didn't want to have to code a whole thing, so regex will have to do.
        if (Instance.TryHandleHashIncrement(message))
            return;

        // If any toast in the queue has the same base message, don't add a duplicate.
        // Instead, update its duration and reset the visible timer so it stays on-screen.
        if (Instance.toastStack.Count > 0)
        {
            string incomingBase = Instance.GetHashBase(message);

            // Convert queue to array to inspect and update items while preserving order
            ToastData[] arr = Instance.toastStack.ToArray();
            bool found = false;
            float newDuration = Instance.defaultDuration + (message.Length * Instance.multPerCharacter);

            for (int i = 0; i < arr.Length; ++i)
            {
                string itemBase = Instance.GetHashBase(arr[i].message);
                if (itemBase == incomingBase)
                {
                    arr[i].duration = newDuration;
                    found = true;
                }
            }

            if (found)
            {
                Instance.LastToastTime = newDuration;

                // Rebuild the queue with the updated items
                Instance.toastStack = new Generic.Queue<ToastData>(arr);

                if (Instance.childCanvas != null)
                {
                    ToastUI toastUI = Instance.childCanvas.GetComponentInChildren<ToastUI>();
                    if (toastUI != null)
                    {
                        toastUI.ResetTimer();
                    }
                }

                return;
            }
        }

        // Find duration of the instance based on message length, then push
        float computedDuration = Instance.defaultDuration + (message.Length * Instance.multPerCharacter);
        Instance.LastToastTime = computedDuration;
        Instance.toastStack.Enqueue(new ToastData(message, substring1, substring2, icon, Instance.LastToastTime));

        // If this is the only toast in the stack, start showing it
        if (Instance.toastStack.Count == 1)
        {
            Instance.StartCoroutine(Instance.TriggerPopIn(Instance.toastPrefab));
        }
    }

    // Methods
    // This allows duplicate detection to ignore incremental numbers.
    private string GetHashBase(string message)
    {
        var hashNumberPattern = new Regex("#(\\d+)");
        if (hashNumberPattern.IsMatch(message))
            return hashNumberPattern.Replace(message, "#", 1);
        return message;
    }

    // Try to handle messages that contain a '#<number>' token by incrementing it
    // NOTE: This is from ChatGPT -- don't worry, I'm not clinically insane for doing this
    private bool TryHandleHashIncrement(string message)
    {
        var hashNumberPattern = new Regex("#(\\d+)");
        if (!hashNumberPattern.IsMatch(message) || toastStack.Count == 0)
            return false;

        // Create a base key by replacing the digit after # with a placeholder so variants can be matched
        string incomingBase = hashNumberPattern.Replace(message, "#", 1);

        // Inspect all queued toasts and update any that match the incoming base
        ToastData[] arr = toastStack.ToArray();
        bool foundAny = false;
        float latestNewDuration = 0f;

        for (int i = 0; i < arr.Length; ++i)
        {
            string itemBase = hashNumberPattern.IsMatch(arr[i].message) ? hashNumberPattern.Replace(arr[i].message, "#", 1) : arr[i].message;
            if (itemBase != incomingBase)
                continue;

            // Determine current number for this item; fallback to incoming if item lacks one
            int current = 0;
            Match itemMatch = hashNumberPattern.Match(arr[i].message);
            if (itemMatch.Success && int.TryParse(itemMatch.Groups[1].Value, out int parsed))
                current = parsed;
            else
            {
                Match incMatch = hashNumberPattern.Match(message);
                if (incMatch.Success) int.TryParse(incMatch.Groups[1].Value, out current);
            }

            int next = current + 1;
            string newMessage = hashNumberPattern.Replace(arr[i].message, "#" + next.ToString(), 1);

            float newDuration = defaultDuration + (newMessage.Length * multPerCharacter);
            latestNewDuration = newDuration;

            arr[i].message = newMessage;
            arr[i].duration = newDuration;
            foundAny = true;
        }

        if (!foundAny)
            return false;

        // Rebuild the queue with updated items
        toastStack = new Generic.Queue<ToastData>(arr);

        // Update last toast time to the last updated duration
        LastToastTime = latestNewDuration;

        // If the currently visible toast matches the incoming base, update its UI and reset timer
        if (childCanvas != null)
        {
            ToastUI toastUI = childCanvas.GetComponentInChildren<ToastUI>();
            if (toastUI != null)
            {
                // Update visible message if it matches the base
                string visibleMessage = toastStack.Peek().message;
                string visibleBase = hashNumberPattern.IsMatch(visibleMessage) ? hashNumberPattern.Replace(visibleMessage, "#", 1) : visibleMessage;
                if (visibleBase == incomingBase)
                {
                    toastUI.UpdateMessage(visibleMessage);
                    toastUI.ResetTimer();
                }
            }
        }

        return true;
    }

    // Called by the visible `ToastUI` when its local timer reaches the end.
    private bool waitingToDequeue = false;
    public void PopCurrentToastFromUI()
    {
        if (waitingToDequeue)
            return;
        waitingToDequeue = true;
        StartCoroutine(TriggerPopOut(0.5f, (go) =>
        {
            // After pop-out completes, remove the toast from the stack and reset state
            if (toastStack.Count > 0)
                toastStack.Dequeue();
            waitingToDequeue = false;
        }));
    }

    private IEnumerator TriggerPopIn(GameObject canvasPrefab)
    {
        // Prevent multiple pop-in coroutines from racing: mark as popping in/animating
        isPoppingIn = true;
        isAnimating = true;

        // Show the popup with the current toast data
        ToastData currentToast = toastStack.Peek();
        yield return StartCoroutine(ShowPopup(canvasPrefab, popInDuration, (toastGO) =>
        {
            // Set the message and icon on the toast prefab
            ToastUI toastUI = toastGO.GetComponent<ToastUI>();
            if (toastUI != null)
            {
                toastUI.Init(this, currentToast.message, currentToast.icon, currentToast.substring1, currentToast.substring2);
            }
            else
            {
                Debug.LogWarning("Toast prefab is missing a ToastUI component to set the message and icon!");
            }

            // Set the toast's anchor position to be top right, with some padding
            windowRect.anchorMin = new Vector2(1f, 1f);
            windowRect.anchorMax = new Vector2(1f, 1f);
            windowRect.pivot = new Vector2(1f, 1f);
            windowRect.offsetMax = new Vector2(-20f, -20f);
            windowRect.offsetMin = new Vector2(-20f, -20f);
        }));
    }
}
