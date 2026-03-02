using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public static Transition Instance { get; private set; }

    public static event Action OnTransition;

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

    private Coroutine fadeCoroutine;

    private Animator animator;

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

        animator = GetComponent<Animator>();
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

        // Get rid of the coroutine reference so we can start new transitions
        fadeCoroutine = null;
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

        // Get rid of the coroutine reference so we can start new transitions
        fadeCoroutine = null;
    }

    private void FadeCoroutineStarter(IEnumerator coroutine)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(coroutine);
    }

    private bool IsInAnimatorTransition()
    {
        return animator.IsInTransition(0) || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    public void TransitionToPlanets(bool fadeText = true)
    {
        if (fadeText && IsInAnimatorTransition())
        {
            // Don't allow starting transitions if we're still moving unless we're skipping the text fade
            return;
        }

        animator.SetBool("PlanetTransition", true);
        currentScreen = Screen.Galaxy;
        OnTransition?.Invoke();
        if (fadeText) FadeCoroutineStarter(FadeTextCoroutine());
        else HideAllText();
        AudioManager.playSound("Fly_By");
    }

    public void TransitionToSettings(bool fadeText = true)
    {
        if (fadeText && IsInAnimatorTransition())
        {
            // Don't allow starting transitions if we're still moving unless we're skipping the text fade
            return;
        }

        animator.SetBool("SettingsTransition", true);
        currentScreen = Screen.Settings;
        OnTransition?.Invoke();
        if (fadeText) FadeCoroutineStarter(FadeTextCoroutine());
        else HideAllText();
    }

    public void TransitionToMainMenuFromPlanets()
    {
        if (IsInAnimatorTransition())
        {
            // Don't allow starting transitions if we're still moving
            return;
        }

        animator.SetBool("PlanetTransition", false);
        currentScreen = Screen.Main;
        OnTransition?.Invoke();
        FadeCoroutineStarter(UnFadeTextCoroutine());
    }

    public void TransitionToMainMenuFromSettings()
    {
        if (IsInAnimatorTransition())
        {
            // Don't allow starting transitions if we're still moving
            return;
        }

        animator.SetBool("SettingsTransition", false);
        currentScreen = Screen.Main;
        OnTransition?.Invoke();
        FadeCoroutineStarter(UnFadeTextCoroutine());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
