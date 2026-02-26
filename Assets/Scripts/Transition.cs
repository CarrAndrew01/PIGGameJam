using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public static Transition Instance { get; private set; }

    public static event Action onTransition;

    public enum Screen
    {
        Main,
        Galaxy,
        Settings,
        None
    }

    public List<TMP_Text> TextObjects = new();

    [ShowInInspector, ReadOnly] private Screen currentScreen = Screen.Main;
    public static Screen CurrentScreen => Instance != null ? Instance.currentScreen : Screen.None;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        switch (GameManager.Instance.intendedScreen)
        {
            case Screen.Main:
                break;
            case Screen.Galaxy:
                TransitionToPlanets(false);
                GameManager.Instance.intendedScreen = Screen.Main;
                break;
            case Screen.Settings:
                TransitionToSettings(false);
                GameManager.Instance.intendedScreen = Screen.Main;
                break;
        }
    }

    private void HideAllText()
    {
        foreach (TMP_Text textObject in TextObjects)
        {
            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0f);
        }
    }

    IEnumerator FadeTextCoroutine()
    {
        for (int i = 0; i < 255; i++)
        {
            foreach (TMP_Text textObject in TextObjects)
            {
                //fade the text out
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, textObject.color.a - 0.04f);

            }

            //the text elements are all firing at the same time so this is safe
            if (TextObjects[0].color.a <= 0f)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UnFadeTextCoroutine()
    {
        for (int i = 0; i < 255; i++)
        {
            foreach (TMP_Text textObject in TextObjects)
            {
                //fade the text in
                //eh, good enough
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, textObject.color.a + 0.01f);
            }

            //the text elements are all firing at the same time so this is safe
            if (TextObjects[0].color.a >= 1f)
            {
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }


    public void TransitionToPlanets(bool fadeText = true)
    {
        gameObject.GetComponent<Animator>().SetBool("PlanetTransition", true);
        currentScreen = Screen.Galaxy;
        onTransition?.Invoke();
        if (fadeText) StartCoroutine(FadeTextCoroutine());
        else HideAllText();
    }

    public void TransitionToSettings(bool fadeText = true)
    {
        gameObject.GetComponent<Animator>().SetBool("SettingsTransition", true);
        currentScreen = Screen.Settings;
        onTransition?.Invoke();
        if (fadeText) StartCoroutine(FadeTextCoroutine());
        else HideAllText();
    }

    public void TransitionToMainMenuFromPlanets()
    {
        gameObject.GetComponent<Animator>().SetBool("PlanetTransition", false);
        currentScreen = Screen.Main;
        onTransition?.Invoke();
        StartCoroutine(UnFadeTextCoroutine());
    }

    public void TransitionToMainMenuFromSettings()
    {
        gameObject.GetComponent<Animator>().SetBool("SettingsTransition", false);
        currentScreen = Screen.Main;
        onTransition?.Invoke();
        StartCoroutine(UnFadeTextCoroutine());
    }
}
