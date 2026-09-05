using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;




public class SceneSwitcher : MonoBehaviour
{
    public const string loadingScreen = "Loading Screen";
    public string sceneToSwitchTo;
    
    public static event  Action CompleteTransition;

    public static void TriggerCompleteTransition() {
  
    }
    private void OnEnable()
    {
        CompleteTransition += StartTransitionSceneCoroutine;
    }
    private void OnDisable()
    {
        CompleteTransition -= StartTransitionSceneCoroutine;
    }

    
    public void BeginSceneTransition(string switchScene)
    {
        // grabs the scene that we actually want to go to
        sceneToSwitchTo = switchScene;
        // sends us to the loading screen
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadingScreen);

    }
    void StartTransitionSceneCoroutine()
    {
        StartCoroutine(TransitionScene());
    }
    // add an event for loading screen to trigger
    IEnumerator TransitionScene()
    {

    
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToSwitchTo);
        // waits for the async scene to fully load
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    // clears the scene we want to swap to so we don't accidentally go to the wrong scene later
    void ClearSceneToSwitchTo()
    {
        sceneToSwitchTo = null;
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     TriggerCompleteTransition();
        // }
    }

}
