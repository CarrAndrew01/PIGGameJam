using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Manages controller-based auto-selection. Has a priority which shifts when the user navigates.
/// Provided by CoPilot, fixed to actually do anything by me.
/// Basically it has a default priority order and then as the player moves between them, the priority shifts to the last selected buttons.
/// </summary>
public class SelectableController : MonoBehaviour
{
    [Header("Selectable Entries")]
    [SerializeField] private List<SelectableEntry> entries = new();

    // Priority order for normal entries. Index 0 = highest priority.
    [ShowInInspector, ReadOnly]
    private readonly List<SelectableEntry> normalOrder = new();
    // Fallback entries: these are ignored for priority reshuffling and are only here as a fallback if nothing else is active.
    [ShowInInspector, ReadOnly]
    private readonly List<SelectableEntry> fallbackOrder = new();

    private bool selectionPending;
    private bool initialised;
    private bool suppressReshuffle;

    private void Awake()
    {
        var sorted = new List<SelectableEntry>(entries);
        sorted.Sort((a, b) => a.initialPriority.CompareTo(b.initialPriority));

        foreach (var entry in sorted)
        {
            if (entry == null) continue;

            if (entry.fallbackOnly)
                fallbackOrder.Add(entry);
            else
                normalOrder.Add(entry);
        }

        initialised = true;
    }

    private void OnEnable()
    {
        if (!initialised) return;

        foreach (var entry in entries)
        {
            if (entry == null) continue;
            entry.OnDeactivated += HandleEntryDeactivated;
            entry.OnSelected += HandleEntrySelected;
            entry.OnDeselected += HandleEntryDeselected;
        }

        Menus.OnMenuStateChanged += HandleMenuStateChanged;
        InputUtils.OnControllerActiveChanged += HandleControllerActiveChanged;

        if (InputUtils.IsControllerActive && !Menus.IsAnyMenuOpen)
            selectionPending = true;
    }

    private void OnDisable()
    {
        foreach (var entry in entries)
        {
            if (entry == null) continue;
            entry.OnDeactivated -= HandleEntryDeactivated;
            entry.OnSelected -= HandleEntrySelected;
            entry.OnDeselected -= HandleEntryDeselected;
        }

        Menus.OnMenuStateChanged -= HandleMenuStateChanged;
        InputUtils.OnControllerActiveChanged -= HandleControllerActiveChanged;
    }

    private void LateUpdate()
    {
        if (!selectionPending) return;
        selectionPending = false;
        ApplyHighestPrioritySelection();
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        if (!menuOpen && InputUtils.IsControllerActive)
            selectionPending = true;
    }

    private void HandleControllerActiveChanged(bool controllerActive)
    {
        if (Menus.IsAnyMenuOpen) return;

        if (controllerActive)
            selectionPending = true;
        else if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void HandleEntryDeactivated()
    {
        if (!InputUtils.IsControllerActive || Menus.IsAnyMenuOpen) return;

        selectionPending = true;
    }

    private void HandleEntrySelected(SelectableEntry entry)
    {
        // Ignore selections made by this controller — only user navigation reshuffles.
        if (suppressReshuffle || !InputUtils.IsControllerActive || Menus.IsAnyMenuOpen) return;

        // Move the selected entry to the front — it becomes highest priority.
        if (normalOrder.Remove(entry))
            normalOrder.Insert(0, entry);
    }

    private void HandleEntryDeselected(SelectableEntry entry) { /* reserved for future use */ }

    private void ApplyHighestPrioritySelection()
    {
        if (EventSystem.current == null) return;

        foreach (var entry in normalOrder)
        {
            if (IsSelectable(entry))
            {
                suppressReshuffle = true;
                EventSystem.current.SetSelectedGameObject(entry.gameObject);
                suppressReshuffle = false;
                return;
            }
        }

        foreach (var entry in fallbackOrder)
        {
            if (IsSelectable(entry))
            {
                suppressReshuffle = true;
                EventSystem.current.SetSelectedGameObject(entry.gameObject);
                suppressReshuffle = false;
                return;
            }
        }
    }

    private static bool IsSelectable(SelectableEntry entry) =>
        entry != null && entry.gameObject.activeInHierarchy && entry.isActiveAndEnabled;
}
