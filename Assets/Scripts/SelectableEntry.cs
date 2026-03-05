using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Companion component for any GameObject with a Selectable (Button, Toggle, whatever.)
/// </summary>
public class SelectableEntry : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Tooltip("Lower value = higher priority. Sets the initial selection order within a SelectableController.")]
    public int initialPriority;

    [Tooltip("If true, this entry never acquires priority through navigation. Only selected as a last-resort fallback.")]
    public bool fallbackOnly;

    public event Action OnDeactivated;
    public event Action<SelectableEntry> OnSelected;
    public event Action<SelectableEntry> OnDeselected;

    private void OnDisable()
    {
        OnDeactivated?.Invoke();
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelected?.Invoke(this);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselected?.Invoke(this);
    }
}
