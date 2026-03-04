using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Astronaut : CatInteract
{
    public override void InteractWithCat()
    {
        base.InteractWithCat();

        // Return to galaxy
        ExitShip();
    }

    public void ExitShip()
    {
        GameManager.Instance.intendedScreen = Transition.Screen.Galaxy;
        SceneManager.LoadScene("Title");
    }
}
