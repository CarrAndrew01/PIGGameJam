using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipExit : MonoBehaviour
{
    public void ExitShip()
    {
        GameManager.Instance.intendedScreen = Transition.Screen.Galaxy;
        SceneManager.LoadScene("Title");
    }
}
