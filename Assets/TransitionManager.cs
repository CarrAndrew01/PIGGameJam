using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set;}
    // Animators to handle transitions
    [SerializeField]
    Animator topAnimator;
    [SerializeField]
    Animator bottomAnimator;
    // Scene variables

    public string sceneToSwitchTo;

    public const string LOADING_SCREEN = "Loading Screen";
    
    // Creates the singleton
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    // function that runs any time a new scene is loaded
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Current Scene: "+scene.name);
        if (scene.name.ToString() == "Loading Screen")
        {
            StartCoroutine(TransitionScene());
        }
    }

    // Starts the process by sending the player to the loading screen
    public void BeginSceneTransition(string switchScene)
    {
        // grabs the scene that we actually want to go to
        sceneToSwitchTo = switchScene;
        // sends us to the loading screen
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LOADING_SCREEN);

    }


    // this is for the loading screen to actually transition screens - waits on loading
    IEnumerator TransitionScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToSwitchTo);
        // waits for the async scene to fully load
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log("Switching to: "+sceneToSwitchTo);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            topAnimator.SetTrigger("TopClose");
            bottomAnimator.SetTrigger("BottomClose");
        } 
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            topAnimator.SetTrigger("TopOpen");
            bottomAnimator.SetTrigger("BottomOpen");
        }       
    }
    
}
