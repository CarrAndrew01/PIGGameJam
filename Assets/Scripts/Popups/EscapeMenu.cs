using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    public void GotoGalaxy()
    {
        GameManager.GotoTitleScreen(Transition.Screen.Galaxy);
    }

    public void GotoSettings()
    {
        GameManager.GotoTitleScreen(Transition.Screen.Settings);
    }

    public void ExitGame()
    {
        GameManager.GotoTitleScreen(Transition.Screen.Main);
    }
}
