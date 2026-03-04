using UnityEngine;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(planetName);
    }
}
