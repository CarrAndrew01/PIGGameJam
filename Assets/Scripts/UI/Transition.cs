using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Transition : MonoBehaviour
{
    public static Transition Instance { get; private set; }

    public static event Action OnTransition;

    public enum Screen
    {
        Main,
        Galaxy,
        FadeToBlackAndCats,
        InstantPlanetsFadeOut, //instantly go to the planet view, then fade the thing out

        Settings,
        None
    }

    public List<TMP_Text> TextObjects = new();
    public List<Selectable> SettingsObjects = new();
    public InputActionReference returnToMainAction;
    public Selectable firstButtonOnPlanetScreen;
    public Selectable firstButtonOnSettingsScreen;
    public Selectable firstButtonOnMainScreen;
    private bool hasSelectedFirstButton = false;

    [ShowInInspector, ReadOnly] private Screen currentScreen = Screen.Main;
    public static Screen CurrentScreen => Instance != null ? Instance.currentScreen : Screen.None;

    [ShowInInspector, ReadOnly]
    private Coroutine fadeCoroutine;
    [ShowInInspector, ReadOnly]
    private Coroutine queueCoroutine;
    [ShowInInspector, ReadOnly]
    private Screen queuedTransition = Screen.None;
    [ShowInInspector, ReadOnly]
    private bool queuedFadeText = true;

    private Animator animator;


    public Image fadeImage;
    private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default ease in/out
 


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

    private void SelectFirstButton()
    {
        if (!InputUtils.IsControllerActive)
            return;
        // Ensure the first button is selected on each screen at the start
        if (currentScreen == Screen.Main && firstButtonOnMainScreen != null)
        {
            firstButtonOnMainScreen.Select();
        }
        else if (currentScreen == Screen.Galaxy && firstButtonOnPlanetScreen != null)
        {
            firstButtonOnPlanetScreen.Select();
        }
        else if (currentScreen == Screen.Settings && firstButtonOnSettingsScreen != null)
        {
            firstButtonOnSettingsScreen.Select();
        }

        hasSelectedFirstButton = true;
    }

    private void OnEnable()
    {
        MenuManager.OnMenuStateChanged += HandleMenuStateChanged;
    }

    private void OnDisable()
    {
        MenuManager.OnMenuStateChanged -= HandleMenuStateChanged;
    }

    private void HandleMenuStateChanged(bool menuOpen)
    {
        // NOTE: For now we are simply turning off buttons when a menu is open to stop automatic navigation
        // TODO: Change menu popups to be explicit navigation and this is no longer needed
        foreach (TMP_Text textObject in TextObjects)
        {
            var btn = textObject.GetComponent<Button>();
            if (btn != null) btn.enabled = !menuOpen;
        }

        foreach (Selectable selectable in SettingsObjects)
        {
            if (selectable != null) selectable.enabled = !menuOpen;
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
                // Bypass the animator guard — nothing meaningful is playing at startup.
                ExecuteTransitionTo(Screen.Galaxy, false);
                GameManager.Instance.intendedScreen = Screen.Main;
                break;
            case Screen.InstantPlanetsFadeOut:
                //first, get the blackout alpha up
                ExecuteTransitionTo(Screen.InstantPlanetsFadeOut, false);
                GameManager.Instance.intendedScreen = Screen.Main;

                break;
            case Screen.Settings:
                ExecuteTransitionTo(Screen.Settings, false);
                GameManager.Instance.intendedScreen = Screen.Main;
                break;
        }
    }

    private void Update()
    {
        if (returnToMainAction.action.WasPressedThisFrame() && !MenuManager.IsAnyMenuOpen)
        {
            switch (currentScreen)
            {
                case Screen.Galaxy:
                    TransitionToMainMenuFromPlanets();
                    break;
                case Screen.Settings:
                    TransitionToMainMenuFromSettings();
                    break;
                default:
                    break;
            }
        }

        // If a controller is active and we haven't selected the first button on the current screen yet, do so.
        if (InputUtils.IsControllerActive)
        {
            if (MenuManager.IsAnyMenuOpen)
            {
                // A menu is open and owns navigation — reset so we re-select when it closes.
                hasSelectedFirstButton = false;
            }
            else if (!hasSelectedFirstButton)
            {
                SelectFirstButton();
            }
        }
        else
        {
            // If a controller isn't active, make sure no buttons are selected
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
            hasSelectedFirstButton = false;
        }
    }

    private void HideAllText()
    {
        foreach (TMP_Text textObject in TextObjects)
        {
            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0f);
            var btn = textObject.GetComponent<Button>();
            if (btn != null) btn.enabled = false;
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

        foreach (TMP_Text textObject in TextObjects)
        {
            var btn = textObject.GetComponent<Button>(); //why this? Why not the gameobject? 
            if (btn != null) btn.gameObject.SetActive(false);
        }

        // Get rid of the coroutine reference so we can start new transitions
        fadeCoroutine = null;
    }

    IEnumerator UnFadeTextCoroutine()
    {
        foreach (TMP_Text textObject in TextObjects)
        {
            var btn = textObject.GetComponent<Button>();
            if (btn != null) btn.gameObject.SetActive(true);
        }

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

    private void QueueTransition(Screen target, bool fadeText)
    {
        if (currentScreen == target) return; // already heading there, don't queue
        queuedTransition = target;
        queuedFadeText = fadeText;
        if (queueCoroutine == null)
            queueCoroutine = StartCoroutine(WaitAndExecuteQueuedTransition());
    }

    // Called by the queue coroutine so it doesn't re-trigger the guard and loop infinitely.
    private void ExecuteTransitionTo(Screen target, bool fadeText)
    {
        switch (target)
        {
            case Screen.Galaxy:
 
                animator.SetBool("PlanetTransition", true);
                currentScreen = Screen.Galaxy;
                OnTransition?.Invoke();
                if (fadeText) FadeCoroutineStarter(FadeTextCoroutine());
                else HideAllText();
                AudioManager.playSound("Fly_By");
                hasSelectedFirstButton = false;
                break;

            case Screen.FadeToBlackAndCats:
                animator.SetBool("PlanetTransition", true);
                OnTransition?.Invoke();

                StartCoroutine(FadeCoroutine(1f, 1.3f, "Cat Ship"));

                AudioManager.playSound("Fly_By");
                hasSelectedFirstButton = false;//not sure what this does
                
                break;

            case Screen.InstantPlanetsFadeOut:
                
                //the problem with using an animation is that alpha is weighted towards one end so it fades to being visible very quickly then takes ages to fade out completely
                //so we reuse the function from introController.
                HideAllText();
                animator.SetTrigger("InstantGalaxy");
                currentScreen = Screen.Galaxy;
                OnTransition?.Invoke();
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f); // Instantly set to fully opaque
                StartCoroutine(FadeCoroutine(0f, 3f));
                hasSelectedFirstButton = false;
                GameManager.Instance.HasSeenIntro = true; // Set the flag to indicate the intro has been seen
                
                break;

            case Screen.Settings:
                animator.SetBool("SettingsTransition", true);
                currentScreen = Screen.Settings;
                OnTransition?.Invoke();
                if (fadeText) FadeCoroutineStarter(FadeTextCoroutine());
                else HideAllText();
                hasSelectedFirstButton = false;
                break;

            case Screen.Main:
                if (currentScreen == Screen.Galaxy)
                animator.SetBool("PlanetTransition", false);
                else if (currentScreen == Screen.Settings)
                animator.SetBool("SettingsTransition", false);
                currentScreen = Screen.Main;
                OnTransition?.Invoke();
                FadeCoroutineStarter(UnFadeTextCoroutine());
                hasSelectedFirstButton = false;
                break;
        }
    }

    IEnumerator WaitAndExecuteQueuedTransition()
    {
        while (IsInAnimatorTransition())
            yield return null;

        Screen target = queuedTransition;
        bool fade = queuedFadeText;
        queuedTransition = Screen.None;
        queueCoroutine = null;

        ExecuteTransitionTo(target, fade);
    }


    public void ChangeScene(string sceneName)
    {
        //change scene to the catship
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

    }


    private IEnumerator FadeCoroutine(float targetAlpha, float fadeDuration, string sceneToLoad = "")
    {
        float startAlpha = fadeImage.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            // Apply custom curve
            float curvedT = fadeCurve.Evaluate(t);
            
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, curvedT);
            
            Color color = fadeImage.color;
            color.a = newAlpha;
            fadeImage.color = color;

            yield return null;
        }

        // Ensure final value
        Color finalColor = fadeImage.color;
        finalColor.a = targetAlpha;
        fadeImage.color = finalColor;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void TransitionToPlanets(bool fadeText = true)
    {
        //if this is the very first time we're going to the planets view, we should skip straight to the cat ship, if there was an intro cutscene it would go here

        if (!GameManager.Instance.HasSeenIntro)
        {
            if (IsInAnimatorTransition())
            {
                QueueTransition(Screen.FadeToBlackAndCats, false);
                return;
            }

            Debug.Log("right bool");

            ExecuteTransitionTo(Screen.FadeToBlackAndCats, false);
            return;

        }


        if (IsInAnimatorTransition())
        {
            QueueTransition(Screen.Galaxy, fadeText);
            return;
        }

        ExecuteTransitionTo(Screen.Galaxy, fadeText);
    }

    public void TransitionToSettings(bool fadeText = true)
    {
        if (IsInAnimatorTransition())
        {
            QueueTransition(Screen.Settings, fadeText);
            return;
        }

        ExecuteTransitionTo(Screen.Settings, fadeText);
    }

    public void TransitionToMainMenuFromPlanets()
    {
        if (IsInAnimatorTransition())
        {
            QueueTransition(Screen.Main, true);
            return;
        }

        ExecuteTransitionTo(Screen.Main, true);
    }

    public void TransitionToMainMenuFromSettings()
    {
        if (IsInAnimatorTransition())
        {
            QueueTransition(Screen.Main, true);
            return;
        }

        ExecuteTransitionTo(Screen.Main, true);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
