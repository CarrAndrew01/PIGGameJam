using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Shared helpers for finding the active input device.
/// Polls each frame using wasPressedThisFrame / ReadValue, which are reliable across scene loads.
/// </summary>
[DefaultExecutionOrder(-100)]
public class InputUtils : MonoBehaviour
{
    [SerializeField] private float gamepadDriftThreshold = 0.2f;
    private static float _driftThreshold = 0.2f;

    private static bool controllerWasLastUsed = false;

    public static event Action<bool> OnControllerActiveChanged;

    private static void SetControllerActive(bool value)
    {
        if (controllerWasLastUsed == value) return;
        controllerWasLastUsed = value;
        Cursor.visible = !value;
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        OnControllerActiveChanged?.Invoke(value);
    }

    [ShowInInspector, ReadOnly]
    public static bool IsControllerActive => controllerWasLastUsed;
    [ShowInInspector, ReadOnly]
    public static bool IsUsingMouse => !controllerWasLastUsed;

    private void Awake()
    {
        _driftThreshold = gamepadDriftThreshold;
    }

    private void Update()
    {
        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;

            bool buttonPressed =
                gp.buttonSouth.wasPressedThisFrame ||
                gp.buttonNorth.wasPressedThisFrame ||
                gp.buttonEast.wasPressedThisFrame ||
                gp.buttonWest.wasPressedThisFrame ||
                gp.startButton.wasPressedThisFrame ||
                gp.selectButton.wasPressedThisFrame ||
                gp.leftShoulder.wasPressedThisFrame ||
                gp.rightShoulder.wasPressedThisFrame ||
                gp.leftTrigger.ReadValue() > _driftThreshold ||
                gp.rightTrigger.ReadValue() > _driftThreshold ||
                gp.dpad.ReadValue().magnitude > _driftThreshold;

            bool stickMoved =
                gp.leftStick.ReadValue().magnitude > _driftThreshold ||
                gp.rightStick.ReadValue().magnitude > _driftThreshold;

            if (buttonPressed || stickMoved)
            {
                SetControllerActive(true);
                return;
            }
        }

        if (Mouse.current != null)
        {
            bool clicked =
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame;
            bool moved = Mouse.current.delta.ReadValue().magnitude > 0.1f;

            if (clicked || moved)
            {
                SetControllerActive(false);
                return;
            }
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
                SetControllerActive(false);
        }
    }
}
