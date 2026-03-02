using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    public void OnClicked()
    {
        GameManager.GotoTitleScreen(Transition.Screen.Galaxy);
    }
}