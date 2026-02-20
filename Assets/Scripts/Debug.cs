using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugTools : MonoBehaviour
{

    private static DebugTools _instance;

    public static DebugTools Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("DebugTools");
                _instance = go.AddComponent<DebugTools>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    void Update()
    {

        //F1 reloads the scene
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        
    }
}
