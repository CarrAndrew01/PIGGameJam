using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using Unity.Mathematics;

public class BlackjackScript : MonoBehaviour
{
    const int CARDNUMMIN = 1;
    const int CARDNUMMAX = 10;
    const int BUSTNUMBER = 21;
    public int currentPlayerValue = 0;
    public int currentDealerValue = 0;

    bool didStand = false;
    bool didBust = false;
    // -1 means no cards placed
    int playerCardIndex = -1;
    int dealerCardIndex = -1;

    public GameObject cardPrefab;

    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> dealerCards = new List<GameObject>();

    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI dealerScoreText;

    public delegate void OnCardsSpawn();
    public static event OnCardsSpawn onCardsSpawn;

    // TODO MAKE DEALER ALWAYS STAND ON 17 OR ABOVE 



    public delegate void Wager();
    public static event Wager wager;
    public delegate void DealCards();
    public static event DealCards dealCards; // push down cards then spread them
    public delegate void Round3();
    public static event Round3 onDeal3;
    public delegate void Round4();
    public static event Round4 onDeal4;
    public delegate void Reset(); // cards return to cat (drags them back?)
    public static event Reset resetEvent;


    public GameObject biddingUI;
    public GameObject gameUI;


    private void OnEnable()
    {
        resetEvent += ResetGame;
        wager += StartWager;
        BidScript.wagerComplete += BeginGame;

    }
    private void OnDisable()
    {
        resetEvent -= ResetGame;
        wager -= StartWager;
        BidScript.wagerComplete -= BeginGame;
    }
    private void Start()
    {
        gameUI.SetActive(false);
        // dealCards?.Invoke();
        // deals 2 cards for player + 1 card for dealer
        // CreateNewCard(true, ref playerCardIndex, ref playerCards);
        // CreateNewCard(true, ref playerCardIndex, ref playerCards);
        // CreateNewCard(false, ref dealerCardIndex, ref dealerCards);
    }
    void BeginGame()
    {
        biddingUI.SetActive(false);
        gameUI.SetActive(true);
        StartCoroutine(BeginningCoroutine());
    }
    IEnumerator BeginningCoroutine()
    {
        yield return new WaitForSeconds(2.5f);
        yield return new WaitForSeconds(1f);
        CreateNewCard(true, ref playerCardIndex, ref playerCards);
        yield return new WaitForSeconds(1f);
        CreateNewCard(true, ref playerCardIndex, ref playerCards);
        yield return new WaitForSeconds(1f);
        CreateNewCard(false, ref dealerCardIndex, ref dealerCards);
    }
    public void Hit()
    {
        if (didBust) return;
        if (didStand) return;
        CreateNewCard(true, ref playerCardIndex, ref playerCards);
    }
    public void Stand()
    {
        didStand = true;
        // if dealer < 17 OR beat player, hit
        while (dealerCardIndex < 4)
        {
            // if dealer score >= 17, break
            if (currentDealerValue >= 17) break;
            CreateNewCard(false, ref dealerCardIndex, ref dealerCards);
        }
    }
    public void CreateNewCard(bool player, ref int cardIndex, ref List<GameObject> cards)
    {
        if (cardIndex < 4) // max of 5 cards (starts at 0)
        {
            // advances cards along index
            cardIndex += 1;
            cards[cardIndex].GetComponent<CardScript>().SetCard();

        }
        // add animate card

        // recalculate score
        int scoreInt = RecalculateScore(cards, cardIndex);
        if (player)
        {
            playerScoreText.text = "Score: " + scoreInt.ToString();
            currentPlayerValue = scoreInt;
            if (currentPlayerValue > BUSTNUMBER)
            {
                didBust = true;
            }
            else if (currentPlayerValue == BUSTNUMBER)
            {
                didStand = true;
            }
        }
        else
        {
            dealerScoreText.text = "Dealer Score: " + scoreInt.ToString();
            currentDealerValue = scoreInt;
        }
        // scoreText.text = "Total - " + test.ToString();
        // scoreText.text += "\nDealer Total - 0";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            resetEvent?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            PayOut();
        }
    }

    public int RecalculateScore(List<GameObject> cards, int cardIndex)
    {
        int scoreCounter = 0;
        int aceCount = 0;
        Debug.Log("index: " + cardIndex);
        for (int i = 0; i <= cardIndex; i++)
        {
            var tempScore = cards[i].GetComponent<CardScript>().GetCardValue();
            if (tempScore != 1)
            {
                scoreCounter += tempScore;
            }
            else
            {
                aceCount++;
            }
        }
        // basically need to check for aces here
        return scoreCounter;
    }
    void ResetGame()
    {
        playerCardIndex = -1;
        dealerCardIndex = -1;
        currentPlayerValue = 0;
        currentDealerValue = 0;
        didStand = false;
        didBust = false;
        // should always be 0
        playerScoreText.text = "Score: " + currentPlayerValue.ToString();
        dealerScoreText.text = "Dealer Score: " + currentDealerValue.ToString();


    }
    void StartWager()
    {
        biddingUI.SetActive(true);
        gameUI.SetActive(false);
    }

    void PayOut()
    {
        GameManager.AdjustMoney(BidScript.bidTokens * BidScript.bidTokenValue);
    }

    // void CreateNewCard(bool player)
    // {
    //     int cardNumber = Random.Range(CARDNUMMIN, CARDNUMMAX);
    //     string newCardValue = cardNumber.ToString();
    //     int newCardSuitIndex = Random.Range(0, 3);
    //     string newCardSuit = SUITS[newCardSuitIndex];

    //     var cardTransform = player ? playerCardsTransform : dealerCardsTransform;

    //     GameObject newCard = Instantiate(cardPrefab, cardTransform);

    //     newCard.GetComponentInChildren<TextMeshProUGUI>().text = newCardValue + newCardSuit;

    //     // animates the cards via event (non-new cards wiggle)
    //     onCardsSpawn?.Invoke();




    //     if (player)
    //     {
    //         currentPlayerValue += cardNumber;
    //         playerCards.Add(newCard);
    //         UpdateValues();


    //         if (currentPlayerValue == BUSTNUMBER)
    //         {
    //             Debug.Log("you win");
    //         }
    //         if (currentPlayerValue > BUSTNUMBER)
    //         {
    //             Bust();
    //         }
    //     }
    //     else
    //     {
    //         currentDealerValue += cardNumber;
    //         dealerCards.Add(newCard);
    //         UpdateValues();

    //         if (currentDealerValue >= BUSTNUMBER)
    //         {
    //             DealerBust();
    //         }
    //     }
    // }
    // void Bust()
    // {
    //     Debug.Log("PLAYER BUST!! Card value: " + currentPlayerValue.ToString());
    //     didBust = true;
    // }
    // void DealerBust()
    // {
    //     Debug.Log("DEALER BUST!! Card value: " + currentDealerValue.ToString());
    // }
    // void UpdateValues()
    // {
    //     string textValue = "Dealer: " + currentDealerValue.ToString() + "\nPlayer: " + currentPlayerValue.ToString();
    //     scoreText.text = textValue;
    // }

    // public void Hit()
    // {
    //     if (didStand) return;
    //     if (didBust) return;
    //     // IF CAN HIT
    //     CreateNewCard(true);
    // }
    // public void Stand()
    // {
    //     if (didStand) return;
    //     if (didBust) return;
    //     didStand = true;


    //     // now dealer plays

    //     //
    //     // if ()
    //     // {

    //     // }
    //     // bust condition
    //     while ((currentDealerValue < 21) && (currentDealerValue < currentPlayerValue))
    //     {
    //         CreateNewCard(false);
    //     }
    //     if (currentDealerValue == currentPlayerValue)
    //     {
    //         Debug.Log("Draw");
    //     }
    //     else if (currentDealerValue > 21)
    //     {
    //         Debug.Log("Dealer Bust");
    //     }
    //     else
    //     {
    //         Debug.Log("Dealer Win");
    //     }
    // }
}
public enum BlackjackStates
{
    WAGER,
    SETTING_UP,
    FLIPPING_FIRST_THREE_CARDS,
    PLAYING_GAME,
    PAYOUT,
    RESET
}
