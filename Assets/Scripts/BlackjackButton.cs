using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class BlackjackButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    TextMeshProUGUI text;
    Button button;

    Color32 textDefaultColor = new Color32(247, 150, 23, 166);
    Color32 textHoverColor = new Color32(255, 255, 255, 166);

    public UnityEvent buttonTarget;

    public bool selectFromStart = false;
    private bool controllerSelectStarted = false;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(() => buttonTarget?.Invoke());
    }

    private void OnEnable()
    {
        controllerSelectStarted = false;
    }

    private void Update()
    {
        bool menuOpen = Menus.IsAnyMenuOpen;

        // Disable/enable the button so controller navigation ignores cats while a menu is open.
        if (button != null && button.enabled == menuOpen)
        {
            button.enabled = !menuOpen;

            if (menuOpen)
            {
                // If this cat was selected, deselect it and clear the outline.
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                    EventSystem.current.SetSelectedGameObject(null);
                controllerSelectStarted = false;
            }
        }

        if (menuOpen) return;

        // Auto-select logic for controller
        if (selectFromStart && InputUtils.IsControllerActive)
        {
            // Only select if nothing is currently selected (avoids stealing focus from other navigables)
            if (!controllerSelectStarted)
            {
                controllerSelectStarted = true;
                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
        else if (!InputUtils.IsControllerActive)
        {
            controllerSelectStarted = false;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Mouse

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null) text.color = textHoverColor;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (text != null) text.color = textDefaultColor;
    }

    // Controller / Keyboard navigation

    public virtual void OnSelect(BaseEventData eventData)
    {
        if (text != null) text.color = textHoverColor;
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        if (text != null) text.color = textDefaultColor;
    }
}
