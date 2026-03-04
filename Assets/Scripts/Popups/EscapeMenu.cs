using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    private void OnEnable()
    {
        SetupNavigation();
    }

    public void GotoGalaxy()
    {
        GameManager.GotoTitleScreen(Transition.Screen.Galaxy);
    }

    public void GotoSettings()
    {
        Menus.Instance.TriggerSettingsMenu();
    }

    public void ExitGame()
    {
        GameManager.GotoTitleScreen(Transition.Screen.Main);
    }

    private void SetupNavigation()
    {
        if (Gamepad.current == null) return;

        // Set up navigation for the buttons in the escape menu
        var buttons = GetComponentInChildren<Button>();
        if (buttons != null)
        {
            EventSystem.current.SetSelectedGameObject(buttons.gameObject);
        }
    }
}
