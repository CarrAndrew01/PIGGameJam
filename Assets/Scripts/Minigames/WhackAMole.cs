using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;


public class WhackAMole : MonoBehaviour
{
    public GameObject background;
    public List<GameObject> circles = new();

    private GameObject currentPopupTarget;

    public int difficulty;//there will be different prefab variants

    //Timer stuff
    private float popDownTimer = 0; //timer. Coroutines are annoying.
    Vector2 timerRange = new(0.8f, 1.3f); //how long the timer should be before we pop down and start again. Randomized for fun.
    float currentTimeTarget = 0; //the current timer target, set in PopUp()
    public float totalGameTimer = 10;



    //Scoring variables
    public int score = 0;
    public int targetScore = 10;
    public bool gameOver = false;


    //UI stuff
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    public TextMeshProUGUI upgradeTimerText;
    public TextMeshProUGUI upgradeScoreText;


    public bool usingJoystick = false;

    public GameObject fakeCursor;
    public InputActionReference action; // expects Button

    FishShadow fishShadow;

    void Awake()
    {
        foreach (Transform childTransform in background.transform)
        {
            if (childTransform.CompareTag("CircleWhack"))
            {
                circles.Add(childTransform.gameObject);
            }
        }

        UpdateScore(0);

        PopUp();

        AdjustDifficulty();
        AdjustBasedOnPlayerUpgrades();

        if(Fishing.LastFishShadow != null){
            fishShadow = Fishing.LastFishShadow;
        }
    }

    void AdjustDifficulty()
    {
        //adjust the timer range based on difficulty

        //wtf you can do this
        //cool syntax
        timerRange = difficulty switch
        {
            1 => new Vector2(0.8f, 1.6f),
            2 => new Vector2(0.6f, 1.1f),
            3 => new Vector2(0.3f, 0.6f),
            4 => new Vector2(0.2f, 0.4f),
            _ => new Vector2(0.8f, 1.3f),
        };

        //adjust the total game timer based on difficulty
        totalGameTimer = difficulty switch
        {
            1 => 15f,
            2 => 12f,
            3 => 10f,
            4 => 8f,
            _ => 10f,
        };
        


    }


    //for reference
    //BASE STATS:
    /*
    1
    1
    1
    1
    0



    */

    void AdjustBasedOnPlayerUpgrades()
    { 
        
        int statCatchSpeed, statCatchArea, statHookGravity, statFishEscapeRate, statHookPullForce;
        
        statCatchSpeed = GameManager.Instance.GetAmountOfUpgrades("CatchSpeed", 1, 0, true);
        statCatchArea = GameManager.Instance.GetAmountOfUpgrades("CatchArea");
        statHookGravity = GameManager.Instance.GetAmountOfUpgrades("Sinker");
        statFishEscapeRate = GameManager.Instance.GetAmountOfUpgrades("EscapeRate");

        //catch speed = amount you need to press to win
        //cant really make this different depending on which upgrades because the score would just get too low
        //so instead its this awkward soltuion lol
        targetScore -= statCatchSpeed;

        //catch area = amount of time total
        totalGameTimer += statCatchArea;

        //fish escape rate = amount of time they stay up for
        //so you get 0.1, 0.1+0.2, 0.1+0.2+0.3 more time depending on upgrade
        timerRange.x += 0.1f * statFishEscapeRate;
        timerRange.y += 0.1f * statFishEscapeRate;

        //damn it I dont wanna get the actual upgrades so I'm just going to put the number in the text box
        //plus it takes up too much room anyway
        //thats my excuse for being lazy 
        if(statCatchSpeed > 0 && upgradeScoreText != null)
        {
            upgradeScoreText.gameObject.SetActive(true);
            upgradeScoreText.text = "- " + statCatchSpeed.ToString() + " Score Required";
        }

        if(statCatchArea > 0 && upgradeTimerText != null)
        {
            upgradeTimerText.gameObject.SetActive(true);
            upgradeTimerText.text = "+ " + statCatchArea.ToString() + " Seconds";
        }
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString() + " / " + targetScore.ToString();
    }

    public bool IsPivotOverButton()
    {
        RectTransform buttonRect = currentPopupTarget.GetComponentInChildren<RectTransform>();

        // Get the world position of the pivot
        Vector3 pivotWorldPos = fakeCursor.transform.position;

        Vector2 localPoint;
        // Convert world/screen point → local point inside button's RectTransform
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            buttonRect,
            pivotWorldPos,                  // or use RectTransformUtility.WorldToScreenPoint(uiCamera, pivotWorldPos) if needed
            null,
            out localPoint))
        {
            // localPoint is now in button's local space (pivot-aware)
            // buttonRect.rect is the local bounds with (0,0) at pivot, extending by size/2 in each direction

            return buttonRect.rect.Contains(localPoint);
        }
        return false;
    }



    void Update()
    {
        if (gameOver)
            return;


        
        //check if we;re using joystick

        //yeah, I'm spaghettying again
        /********
            basically there's a fancy unity thing called a virtual mouse that I could use here
            but I don't have time right now

            TODO: make it work betterer
        ******/

        //the fake cursor always follows the mouse and vice versa
        //never mind that royally fucks everything up, we'll just move the mouse when its time

        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            //Read the current value of the left stick as a Vector2
            Vector3 moveDirection = gamepad.leftStick.ReadValue();

            if (Vector2.Distance(moveDirection, new Vector2(0, 0)) > 0.001f)
            {
                //basically, we now know we're using the joystick
                if (!usingJoystick)
                {
                    usingJoystick = true;
                    Cursor.visible = false;
                    fakeCursor.SetActive(true);

                    Vector2 pos;

                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    GetComponentInParent<Canvas>().transform as RectTransform,
                    Input.mousePosition,
                    GetComponentInParent<Canvas>().worldCamera,   // null works for Screen Space - Overlay
                    out pos))
                    {
                        fakeCursor.GetComponent<RectTransform>().localPosition = pos;
                    }
                }
                fakeCursor.transform.position += moveDirection * 2;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            Vector3 mouseDirection = mouse.delta.ReadValue();
            //if we move the mouse, just have the other cursor follow it around 
            if (Vector2.Distance(mouseDirection, new Vector2(0, 0)) > 0.001f)
            {
                if (usingJoystick)
                {
                    usingJoystick = false;
                    Cursor.visible = true;
                    fakeCursor.SetActive(false);
                }
            }
        }

        if (usingJoystick && IsPivotOverButton() && action.action.WasPressedThisFrame())
        {
            OnClicked(currentPopupTarget); //for joystick
        }


        //timers and such
        popDownTimer += Time.deltaTime;
        totalGameTimer -= Time.deltaTime;

        string gameTimerRounded = totalGameTimer.ToString("F1");
        timerText.text = gameTimerRounded;

        if (totalGameTimer <= 0)
        {
            fishShadow.EndFishing(false);
            gameOver = true;
        }

        if (popDownTimer > currentTimeTarget)
        {
            //we failed, pop down and start again
            Debug.Log("Fail!");
            currentPopupTarget.GetComponentInParent<Animator>().SetBool("PopUp", false);
            PopUp();
        }
    }

    public void OnClicked(GameObject go)
    {
        if (go.transform.parent.gameObject != currentPopupTarget)
        {
            return;
        }

        currentPopupTarget.GetComponentInParent<Animator>().SetBool("PopUp", false);
        UpdateScore(1);

        if (score >= targetScore)
        {
            Debug.Log("we win!");
            fishShadow.EndFishing(true);

            //Destroy(this.gameObject);
        }
        else
        {
            PopUp();
        }
    }



    //Randomly select which one will pop up, then play the animation
    void PopUp()
    {
        RandomizeSelection();

        currentPopupTarget.GetComponentInParent<Animator>().SetBool("PopUp", true);
        popDownTimer = 0f;
        currentTimeTarget = Random.Range(timerRange.x, timerRange.y);
    }

    void RandomizeSelection()
    {
        int rand = Random.Range(0, circles.Count);

        while (circles[rand].transform.GetChild(0).gameObject == currentPopupTarget)
        {
            rand = Random.Range(0, circles.Count); //keep rolling until we dont have the same one that we just clicked
        }

        currentPopupTarget = circles[rand].transform.GetChild(0).gameObject;
    }
}
