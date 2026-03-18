using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// <summary>
/// Utility class for handling resolution changes and fullscreen toggle checking.
/// Should be placed on a persistent GameObject (GameManager, probably)
/// </summary>
public class ResolutionUtils : MonoBehaviour
{
    public static event Action<Resolution> OnResolutionChanged;
    public static event Action<bool> OnFullscreenToggled;

    [Header("State")]
    [ShowInInspector, ReadOnly]
    private Resolution currentResolution;
    [ShowInInspector, ReadOnly]
    private bool isFullscreen;

    private void Start()
    {
        currentResolution = Screen.currentResolution;
        isFullscreen = Screen.fullScreen;
    }

    private void LateUpdate()
    {
        // Check for resolution change
        if (Screen.currentResolution.width != currentResolution.width ||
            Screen.currentResolution.height != currentResolution.height)
        {
            currentResolution = Screen.currentResolution;
            OnResolutionChanged?.Invoke(currentResolution);
        }

        // Check for fullscreen toggle
        if (Screen.fullScreen != isFullscreen)
        {
            isFullscreen = Screen.fullScreen;
            OnFullscreenToggled?.Invoke(isFullscreen);
        }
    }
}
