using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to a planet Button. Manages hover/select visuals and scene entry.
/// Wire the Button's onClick to TryEnter(). Set Button Navigation to point
/// at neighbouring planets so the EventSystem handles controller navigation.
/// </summary>
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

    private void Awake()
    {
        SetVisualsActive(false);
    }

    // --- Mouse ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetVisualsActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetVisualsActive(false);
    }

    // --- Controller / Keyboard navigation ---

    public void OnSelect(BaseEventData eventData)
    {
        SetVisualsActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetVisualsActive(false);
    }

    // --- Entry (called by Button.onClick) ---

    /// <summary>
    /// Wire this to the planet Button's onClick event.
    /// </summary>
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
