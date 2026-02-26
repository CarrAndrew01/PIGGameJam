using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    public void GotoGalaxy()
    {
        GameManager.GotoTitleScreen(Transition.IntendedScreen.Galaxy);
    }

    public void GotoSettings()
    {
        GameManager.GotoTitleScreen(Transition.IntendedScreen.Settings);
    }

    public void ExitGame()
    {
        GameManager.GotoTitleScreen(Transition.IntendedScreen.Main);
    }
}
