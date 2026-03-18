using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlanetHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Planet Data")]
    public string planetName;
    public StatType requiredEntryStat;
    public bool CanEnter => GameManager.GetPlayerStat(requiredEntryStat) >= 1f;

    [Header("Visuals")]
    public GameObject arrow;
    public GameObject nameLabel;
    public GameObject highlight;

    private Button button;

    private void Awake()
    {
        SetVisualsActive(false);
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        MenuManager.OnMenuStateChanged += HandleMenuStateChanged;

        // Sync to current state in case events were missed while disabled
        if (button != null)
            button.enabled = !MenuManager.IsAnyMenuOpen;
    }

    private void OnDisable()
    {
        MenuManager.OnMenuStateChanged -= HandleMenuStateChanged;
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        if (button != null)
            button.enabled = !menuOpen;

        if (menuOpen)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                EventSystem.current.SetSelectedGameObject(null);
            SetVisualsActive(false);
        }
    }

    // Mouse

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetVisualsActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetVisualsActive(false);
    }

    // Controller / Keyboard navigation

    public void OnSelect(BaseEventData eventData)
    {
        SetVisualsActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetVisualsActive(false);
    }

    // Entry (called by Button.onClick)
    public void TryEnter()
    {
        if (CanEnter)
        {
            PlanetSelection.Instance.GoToPlanetScene(planetName);
        }
        else
        {
            Toast.ShowToast("Your ship can't go there yet!");
        }
    }

    private void SetVisualsActive(bool active)
    {
        if (arrow != null)     arrow.SetActive(active);
        if (nameLabel != null) nameLabel.SetActive(active);
        if (highlight != null) highlight.SetActive(active);
    }
}
