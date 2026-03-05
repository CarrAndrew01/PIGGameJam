using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class BlackjackButton : MonoBehaviour
{
    Button button;

    Color32 textDefaultColor = new Color32(247, 150, 23, 166);
    Color32 textHoverColor = new Color32(255, 255, 255, 166);

    public UnityEvent buttonTarget;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => buttonTarget?.Invoke());
            // Set colors because why didn't Jack just make buttons to begin with
            button.colors = new ColorBlock
            {
                normalColor = textDefaultColor,
                highlightedColor = textHoverColor,
                pressedColor = textHoverColor,
                selectedColor = textHoverColor,
                disabledColor = new Color32(247, 150, 23, 100),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
             };
        }
            
    }

    private void OnEnable()
    {
        Menus.OnMenuStateChanged += HandleMenuStateChanged;

        // Sync to current state in case events were missed while disabled
        if (button != null)
            button.enabled = !Menus.IsAnyMenuOpen;
    }

    private void OnDisable()
    {
        Menus.OnMenuStateChanged -= HandleMenuStateChanged;
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        if (button != null)
            button.enabled = !menuOpen;

        if (menuOpen && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }
}
