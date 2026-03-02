using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{

    int suit;
    int num;

    public Image suitTL;
    public Image suitBR;
    public TextMeshProUGUI numberText;

    public List<Sprite> SUITS = new List<Sprite>();

    public Animator cardAnimator;
    Vector2 cardPilePosition;
    public Vector2 intendedPosition;
    [SerializeField]
    Vector2 currentMoveTarget;
    public bool moving = false;
    public GameObject catHandPrefab;

    float maxSpeed = 1000;

    private void OnEnable()
    {
        BlackjackScript.dealCards += MoveToCardPos;
        BlackjackScript.resetEvent += ResetCard;
    }
    private void OnDisable()
    {
        BlackjackScript.dealCards -= MoveToCardPos;
        BlackjackScript.resetEvent -= ResetCard;
    }
    private void Start()
    {
        cardAnimator = GetComponent<Animator>();
        cardPilePosition = transform.localPosition;
        currentMoveTarget = intendedPosition;
    }
    public void SetCard()
    {
        suit = Random.Range(0, 3);
        num = Random.Range(1, 13);
        string numString;

        suitTL.sprite = SUITS[suit];
        suitBR.sprite = SUITS[suit];

        switch (num)
        {
            case 11:
                numString = "J";
                break;
            case 12:
                numString = "Q";
                break;
            case 13:
                numString = "K";
                break;
            default:
                numString = num.ToString();
                break;
        }
        if (numberText != null)
        {
            numberText.text = numString;
        }
        // paw fish
        if (suit <= 1)
        {
            numberText.color = new Color32(0, 0, 0, 235);

        }
        // cat heart
        else
        {
            numberText.color = new Color32(184, 67, 48, 235);
        }
        StartCoroutine(AnimateFlip());
        var catHand = Instantiate(catHandPrefab, transform.parent.parent.parent);
        catHand.transform.position = transform.position + new Vector3(0, 210, 0);
    }
    IEnumerator AnimateFlip()
    {
        yield return new WaitForSeconds(.5f);
        cardAnimator.SetTrigger("Flip");
        string flipSound = "Card_Flip" + Random.Range(1, 3).ToString();
        AudioManager.playSound?.Invoke(flipSound);
    }
    public int GetCardValue()
    {
        // everything after 10 is counted as 10. aces are calculated in blackjack script, return as 1
        if (num >= 10) return 10;
        // return 1-9
        return num;
    }

    // public Vector2 cardPilePosition;
    // public Vector2 intendedPosition;


    // slowly moves cards out to position
    void Movement()
    {
        float step = (2f * Vector2.Distance(transform.localPosition, currentMoveTarget)) * Time.deltaTime;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentMoveTarget, step);
        if (Vector3.Distance(transform.localPosition, currentMoveTarget) < 0.001f)
        {
            moving = false;
        }
    }
    private void Update()
    {
        Movement();
    }

    void MoveToCardPos()
    {
        currentMoveTarget = intendedPosition;
    }
    void ResetCard()
    {
        cardAnimator.SetTrigger("Reset");
        StartCoroutine(resetTrigger());
    }
    // fixes an issue where triggers were held
    IEnumerator resetTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        cardAnimator.ResetTrigger("Reset");
        currentMoveTarget = cardPilePosition;
    }

}
