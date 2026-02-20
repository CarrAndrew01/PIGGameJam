using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WhackAMole : MonoBehaviour
{

    List<GameObject> circles = new();

    GameObject currentPopupTarget;
    [SerializeField]


    //Timer stuff
    float popDownTimer = 0; //timer. Coroutines are annoying.
    Vector2 timerRange = new(0.5f, 1.2f); //how long the timer should be before we pop down and start again. Randomized for fun.
    float currentTimeTarget = 0; //the current timer target, set in PopUp()
    float totalGameTimer = 10f;



    //Scoring variables
     public int target = 5;
     public int score = 0;
     public int targetScore = 5;

    //UI stuff
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;


    void Awake()
    {
        foreach (Transform childTransform in transform)
        {
            if (childTransform.CompareTag("CircleWhack"))
            {
                circles.Add(childTransform.gameObject);
            }
        } 
 
        StartCoroutine(TotalTimer());
        PopUp();
    }

    IEnumerator TotalTimer()
    {
        yield return new WaitForSeconds(totalGameTimer);
        Debug.Log("Game Over! Final Score: " + score);
        Destroy(this.gameObject);
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString() + " / " + targetScore.ToString();
    }

    void Update()
    {
        popDownTimer += Time.deltaTime;

        totalGameTimer -= Time.deltaTime;

        string gameTimerRounded = totalGameTimer.ToString("F1"); 
        timerText.text = gameTimerRounded;
        
        if (Input.GetMouseButtonDown(0))
        {
            // Convert mouse position from screen space to world space
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Cast a 2D ray from that world point with zero direction/distance (detects colliders at the origin)
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            // Check if something was hit
            if (hit.collider != null && hit.collider.gameObject == currentPopupTarget   )
            {
                currentPopupTarget.GetComponentInParent<Animator>().SetBool("PopUp", false);
                UpdateScore(1);

                if(score >= targetScore)
                {
                    Debug.Log("we win!");
                    Destroy(this.gameObject);
                }
                else
                {
                    PopUp();
                }
            }
        }

        if(popDownTimer > currentTimeTarget)
        {
            //we failed, pop down and start again
            Debug.Log("Fail!");
            currentPopupTarget.GetComponentInParent<Animator>().SetBool("PopUp", false);
            PopUp();
        }
    }

    //Randomly select which one will pop up, then play the animation
    void PopUp()
    {
        int rand = Random.Range(0, circles.Count);
        currentPopupTarget = circles[rand].transform.GetChild(0).gameObject;

        currentPopupTarget.GetComponentInParent<Animator>().SetBool("PopUp", true);
        popDownTimer = 0f;
        currentTimeTarget = Random.Range(timerRange.x, timerRange.y);

    }
}
