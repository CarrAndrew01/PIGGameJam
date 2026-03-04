using UnityEngine.InputSystem;

/// <summary>
/// Shared helpers for finding the active input device.
/// </summary>
public static class InputUtils
{
    /// <summary>
    /// Returns true if the most recently used device was a gamepad.
    /// Just compares the last update timestamps of the current Gamepad, Mouse, and Keyboard and returns the latest.
    /// </summary>
    public static bool IsControllerActive()
    {
        if (Gamepad.current == null) return false;

        double mouseTime    = Mouse.current?.lastUpdateTime    ?? double.MinValue;
        double keyTime      = Keyboard.current?.lastUpdateTime ?? double.MinValue;
        double gamepadTime  = Gamepad.current.lastUpdateTime;

        return gamepadTime > mouseTime && gamepadTime > keyTime;
    }
}
