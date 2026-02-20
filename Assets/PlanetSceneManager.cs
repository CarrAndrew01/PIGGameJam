using UnityEngine;

public class PlanetSceneManager : MonoBehaviour
{


    public void GoToPlanetScene(string planetName)
    {   
        Debug.Log("Going to planet scene: " + planetName);
        //load the planet scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(planetName);
    }

    
}
