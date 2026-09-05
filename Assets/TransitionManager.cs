using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set;}
    // Animators to handle transitions
    [SerializeField]
    Animator topAnimator; // top of black bars
    [SerializeField]
    Animator bottomAnimator; // bottom of black bars
    PlayableDirector loadingScreenTimeline; // exists to catch the values of the loading screen being done
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
            // maybe grab the animator here?
            GameObject loadingScreenTimelineObject = GameObject.Find("Timeline");
            if (loadingScreenTimelineObject != null)
            {
                loadingScreenTimeline = loadingScreenTimelineObject.GetComponent<PlayableDirector>();
                StartCoroutine(TransitionScene());
            }
        }
        TriggerOpen();
    }

    // Starts the process by sending the player to the loading screen
    public void BeginSceneTransition(string switchScene)
    {
        TriggerClose();
        // grabs the scene that we actually want to go to
        sceneToSwitchTo = switchScene;
        // sends us to the loading screen
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LOADING_SCREEN);

    }


    // this is for the loading screen to actually transition screens - waits on loading
    IEnumerator TransitionScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToSwitchTo);
        asyncLoad.allowSceneActivation = false; // scene can't load until we let it
        // waits for the async scene to fully load
        bool triggeredTransition = false;

        while (!asyncLoad.isDone)
        {
            double remainingTime = loadingScreenTimeline.duration - loadingScreenTimeline.time;
            bool animationComplete = remainingTime <= 0;

            if (!triggeredTransition && remainingTime <= 0.6f)
            {
                TriggerClose();
                triggeredTransition = true;
            }
            if (asyncLoad.progress >= 0.9f && animationComplete)
            {
                asyncLoad.allowSceneActivation = true;
            }

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
            TriggerClose();
        } 
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            TriggerOpen();
        }       
    }
    void TriggerClose()
    {
        topAnimator.SetTrigger("TopClose");
        bottomAnimator.SetTrigger("BottomClose");
    }
    void TriggerOpen()
    {
        topAnimator.SetTrigger("TopOpen");
        bottomAnimator.SetTrigger("BottomOpen");
    }
    
}
