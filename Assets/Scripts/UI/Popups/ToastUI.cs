using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ToastUI : ListItem
{
    // State
    private float timer = 0f; // Time remaining for the toast to be visible, in seconds

    [Header("Toast UI Settings")]
    public Color startTimerColor = new Color(1f, 1f, 1f);
    public Color endTimerColor = new Color(1f, 1f, 1f);

    [Header("Toast UI Components")]
    public Slider timerBar;

    private Toast toastParent;
    private bool popRequested = false;

    public void Init(Toast parent, string name, Sprite iconSprite = null, string subtext = "", string subtext2 = "", string description = "", string mechanicalDescription = "", int index = -1)
    {
        string displayName = RemoveHashForDisplay(name);
        base.Init(null, displayName, iconSprite, subtext, subtext2, description, mechanicalDescription, index);
        toastParent = parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerBar != null && toastParent != null)
        {
            timer += Time.deltaTime;

            float duration = Mathf.Max(0.0001f, toastParent.LastToastTime);
            float progress = Mathf.Clamp01(timer / duration);
            timerBar.value = progress;

            // Update the timer bar color based on progress
            timerBar.fillRect.GetComponent<Image>().color = Color.Lerp(startTimerColor, endTimerColor, progress);

            // When progress reaches the end, notify parent to pop out this toast
            if (!popRequested && progress >= 1f)
            {
                popRequested = true;
                toastParent?.PopCurrentToastFromUI();
            }
        }
    }
    
    // Resets the internal timer so the visible toast will stay on-screen
    public void ResetTimer()
    {
        timer = 0f;
        popRequested = false;
    }

    // Update the visible message text
    public void UpdateMessage(string newMessage)
    {
        if (nameField != null)
            nameField.text = RemoveHashForDisplay(newMessage);
    }

    // Remove the '#' before numbers for display, e.g. "You have #1" -> "You have 1"
    private string RemoveHashForDisplay(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;
        var r = new Regex("#(\\d+)");
        if (r.IsMatch(message))
            return r.Replace(message, "$1", 1);
        return message;
    }
}
