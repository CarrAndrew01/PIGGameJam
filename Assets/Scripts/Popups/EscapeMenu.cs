using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
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
}
