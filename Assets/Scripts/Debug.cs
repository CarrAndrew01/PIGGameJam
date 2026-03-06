using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugTools : MonoBehaviour
{
#if UNITY_EDITOR
    public static readonly bool DEBUG_ENABLED = true;
#else
    public static readonly bool DEBUG_ENABLED = false;
#endif

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
        // F2 breaks the editor
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Break();
        }
        // F3 logs the current scene name
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);
        }
        // F4 adds 100 money to the player
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            GameManager.AdjustMoney(100);
        }
        // F5 adds 10 temporary fish to the player
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            CaughtFish tempFish = new CaughtFish
            {
                fish = GameManager.Instance.TEMPFISH,
                weight = 1f,
                planetOfOrigin = "/DEBUG"
            };

            for (int i = 0; i < 10; i++)
            {
                GameManager.AddFishToInventory(tempFish, ignoreLimit: true);
            }
        }
    }
}
