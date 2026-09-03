using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlanetSelection : MonoBehaviour
{
    public static PlanetSelection Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void GoToPlanetScene(string planetName)
    {
        // StartCoroutine(AsyncLoadScene(planetName));;
        UnityEngine.SceneManagement.SceneManager.LoadScene(planetName);
    }
    IEnumerator AsyncLoadScene(string planetName)
    {
        planetName = "Loading Screen";
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(planetName);
        // waits for the async scene to fully load
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
