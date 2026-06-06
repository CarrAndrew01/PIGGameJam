using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//only doing this so I can have the little icons for each cat/speaker
[System.Serializable]
public class Dialogue
{
    public Sprite icon;
    public string str;
}

public class IntroController : MonoBehaviour
{
    //I was going to be fancy and make this a queue but of course you can't serialize them like everything else in fucking unity
    [SerializeField]
    public List<Dialogue> introText;

    public int introIndex = 0;


    public enum State
    {
        Delay,
        ReadyForNext,
        Finished
    }

    public State state = State.Delay; 

    public float timer = 0f;
    public float delay = 0.8f;


    #region SLOOOOOOOOPPPPPPPPP SLOP SLOP SLOP SLOP SLOP SLOPPPPPPPP SLOP
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Default ease in/out
    
    public float fadeDuration = 1.5f;
    private Coroutine currentFade;
    public Image fadeImage;

    public Image imgIcon;

    public TextMeshProUGUI tmp;

    public InputActionReference selectAction; // expects Vector2, only the x component is used for left/right movement

    public GameObject dialogueBox;
    public GameObject exitBox;

    public void FadeTo(float targetAlpha, float duration = -1f, bool sceneChange = false)
    {
        if (duration > 0) fadeDuration = duration;
        
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeCoroutine(targetAlpha, sceneChange));
    }

    private IEnumerator FadeCoroutine(float targetAlpha, bool sceneChange)
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

        if(sceneChange)
        {
            GameManager.Instance.intendedScreen = Transition.Screen.InstantPlanetsFadeOut;
            SceneManager.LoadScene("Title");
        }

    }
    #endregion

    void NextDialogue(){
        tmp.text = introText[introIndex].str; //set the first as-is
        imgIcon.sprite = introText[introIndex].icon;
    }

    void Update()
    {
        //when a little bit of time has passed, players can now press the next button to skip dialogue (enter? the confirm button anyway)
        //check the input and go to the next queue
        
        if(state == State.Delay)
        {
            timer+=Time.deltaTime;
            if(timer > delay)
            {
                if (introIndex >= introText.Count - 1)
                {
                    state = State.Finished;
                    //we're at the end of the dialogue, so we can go back to the galaxy map and start the game proper
                    FadeTo(1f, 3f, true);
                    //GameManager.Instance.HasSeenIntro = true;


                }
                else
                {
                    state = State.ReadyForNext;
                    timer = 0;
                }
            }
        }

        if(state == State.ReadyForNext)
        {
            if(selectAction.action.WasPressedThisFrame())
            {
                //go to the next bit of dialogue
                tmp.text = "";
                introIndex++;

                NextDialogue();
                state = State.Delay;
            }
        }
    }


    //need to turn off the cat shops, stop you hovering over them in general
    //might have been easier to make a new scene actually
    //but then any other changes would have to be cloned over so idk

    [SerializeField]
    public List<GameObject> Interact = new();

    void DisableAllButtons()
    {
        foreach (GameObject button in Interact)
        {
            if(button.TryGetComponent(out MonoBehaviour btn)){
                btn.enabled = false;
            }
            if(button.TryGetComponent(out Button uiBtn)){
                uiBtn.enabled = false;
            }
        }
    }

    public Animator dialogueAnimator;

    void Start()
    {
        if(GameManager.Instance.HasSeenIntro) return;

        //quickly throw the black up, then fade it back out
        if(gameObject.GetComponent<Image>() == null)
        {
            //shit, just disable the script, it has no other purpose
            this.enabled = false;
            return;
        }

        //disable the buttons
        DisableAllButtons();

        if (dialogueAnimator != null)
        {
            dialogueAnimator.SetBool("Expand", true);
        }

        gameObject.GetComponent<Image>().enabled = true;

        dialogueBox.SetActive(true);
        exitBox.SetActive(false);

        FadeTo(0f, 3f);
        NextDialogue();
    }
}
