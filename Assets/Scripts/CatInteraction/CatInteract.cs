using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CatInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public Image outlineImage;
    public Image pointerImage;
    Color startColour;

    public CatSound catsound = CatSound.Medium_Meow;

    public bool selectFromStart = false;
    private bool controllerSelectStarted = false;

    private Button button;

    public virtual void Start()
    {
        startColour = outlineImage.color;
        button = GetComponent<Button>();

        // Sync button state now that button is initialized
        if (button != null)
            button.enabled = !Menus.IsAnyMenuOpen;

        // If the controller was already active when this scene loaded, no event will fire so we do it here
        if (selectFromStart && InputUtils.IsControllerActive && !Menus.IsAnyMenuOpen && !controllerSelectStarted)
        {
            controllerSelectStarted = true;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    protected virtual void OnEnable()
    {
        controllerSelectStarted = false;
        Menus.OnMenuStateChanged += HandleMenuStateChanged;
        InputUtils.OnControllerActiveChanged += HandleControllerActiveChanged;
    }

    protected virtual void OnDisable()
    {
        Menus.OnMenuStateChanged -= HandleMenuStateChanged;
        InputUtils.OnControllerActiveChanged -= HandleControllerActiveChanged;
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        if (button != null)
            button.enabled = !menuOpen;

        if (menuOpen)
        {
            // If this cat was selected, deselect it and clear the outline.
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                EventSystem.current.SetSelectedGameObject(null);
            OutlineCat(false);
            controllerSelectStarted = false;
        }
        else if (selectFromStart && InputUtils.IsControllerActive && !controllerSelectStarted)
        {
            // Menu just closed and controller is active — re-select if appropriate
            controllerSelectStarted = true;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    private void HandleControllerActiveChanged(bool controllerActive)
    {
        if (Menus.IsAnyMenuOpen) return;

        if (controllerActive)
        {
            if (selectFromStart && !controllerSelectStarted)
            {
                controllerSelectStarted = true;
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
        else
        {
            controllerSelectStarted = false;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // Mouse

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        OutlineCat(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        OutlineCat(false);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Interact();
    }

    // Controller / Keyboard navigation

    public void OnSelect(BaseEventData eventData)
    {
        OutlineCat(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OutlineCat(false);
    }

    public void Interact()
    {
        InteractWithCat();
        switch (catsound)
        {
            case CatSound.High_Meow:
                AudioManager.playSound?.Invoke("High_Meow");
                break;
            case CatSound.Medium_Meow:
                AudioManager.playSound?.Invoke("Medium_Meow");
                break;
            case CatSound.Low_Meow:
                AudioManager.playSound?.Invoke("Low_Meow");
                break;
        }
    }

    public virtual void InteractWithCat() { }

    void OutlineCat(bool on)
    {
        var alpha = on ? 1 : 0;

        if (outlineImage != null)
            outlineImage.color = new Color(startColour.r, startColour.g, startColour.b, alpha);

        if (pointerImage != null)
            pointerImage.color = new Color(startColour.r, startColour.g, startColour.b, alpha);
    }
}

public enum CatSound
{
    High_Meow,
    Medium_Meow,
    Low_Meow,
    No_Sound
}
