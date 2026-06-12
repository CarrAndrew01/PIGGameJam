using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class ShipExit : MonoBehaviour
{
    public CanvasGroup exitButtonCanvasGroup;
    public Button exitButton;
    public float fadeDuration = 0.5f;

    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    private void Start()
    {
        exitButtonCanvasGroup.alpha = 0f; // Start with the exit button hidden
        exitButton.enabled = false; // Disable the button until it's visible
    }

    public void ExitShip()
    {
        GameManager.Instance.intendedScreen = Transition.Screen.InstantPlanetsFadeOut;
        SceneManager.LoadScene("Title");
    }

    public void Update()
    {
        // Check if the player is using mouse \ and show the exit button if they are
        if (InputUtils.IsUsingMouse)
        {
            // Show the exit button
            if (fadeOutCoroutine != null) { StopCoroutine(fadeOutCoroutine); fadeOutCoroutine = null; }
            if (fadeInCoroutine == null && exitButtonCanvasGroup.alpha < 1f)
                fadeInCoroutine = StartCoroutine(FadeInCanvasGroup(exitButtonCanvasGroup, fadeDuration));
            exitButton.enabled = true;
        }
        else
        {
            // Hide the exit button
            if (fadeInCoroutine != null) { StopCoroutine(fadeInCoroutine); fadeInCoroutine = null; }
            if (fadeOutCoroutine == null && exitButtonCanvasGroup.alpha > 0f)
                fadeOutCoroutine = StartCoroutine(FadeOutCanvasGroup(exitButtonCanvasGroup, fadeDuration));
        }
    }

    // Coroutine to fade out the canvas group
    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        exitButton.enabled = false;
        fadeOutCoroutine = null; // Reset the coroutine reference so it can be started again
    }

    // Coroutine to fade in the canvas group
    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, time / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        fadeInCoroutine = null;
    }
}
