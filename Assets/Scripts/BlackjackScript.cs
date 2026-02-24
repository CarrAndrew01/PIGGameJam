using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
    int cardIndex = 0;

    public GameObject cardPrefab;

    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> dealerCards = new List<GameObject>();

    public TextMeshProUGUI scoreText;

    public delegate void OnCardsSpawn();
    public static event OnCardsSpawn onCardsSpawn;

    void CreateNewCard(bool player)
    {
        if (cardIndex <= 4) // max of 5 cards (starts at 0)
        {
            playerCards[cardIndex].GetComponent<CardScript>().SetCard();
            // advances cards along index
            cardIndex += 1;
        }
        // add animate card

        // recalculate score
        var test = RecalculateScore(playerCards);
        Debug.Log("Score: " + test);
        scoreText.text = "Total - " + test.ToString();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateNewCard(true);
        }
    }

    public int RecalculateScore(List<GameObject> cards)
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
