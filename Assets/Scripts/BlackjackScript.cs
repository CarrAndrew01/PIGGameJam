using System;
using System.Collections.Generic;
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

    String[] SUITS = new string[] { "♥", "♦", "♣", "♠" };

    public GameObject cardPrefab;
    public Transform dealerCardsTransform;
    public Transform playerCardsTransform;

    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> dealerCards = new List<GameObject>();

    public TextMeshProUGUI scoreText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cardPrefab != null)
        {
            CreateNewCard(true);
            CreateNewCard(true);
            CreateNewCard(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        // hit
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Hit();
        }
        // stand
        if (Input.GetKeyDown(KeyCode.E))
        {
            Stand();
        }
    }
    void CreateNewCard(bool player)
    {
        int cardNumber = Random.Range(CARDNUMMIN, CARDNUMMAX);
        string newCardValue = cardNumber.ToString();
        int newCardSuitIndex = Random.Range(0, 3);
        string newCardSuit = SUITS[newCardSuitIndex];

        var cardTransform = player ? playerCardsTransform : dealerCardsTransform;

        GameObject newCard = Instantiate(cardPrefab, cardTransform);

        newCard.GetComponentInChildren<TextMeshProUGUI>().text = newCardValue + newCardSuit;

        if (player)
        {
            currentPlayerValue += cardNumber;
            playerCards.Add(newCard);
            UpdateValues();

            if (currentPlayerValue > BUSTNUMBER)
            {
                Bust();
            }
        }
        else
        {
            currentDealerValue += cardNumber;
            dealerCards.Add(newCard);
            UpdateValues();

            if (currentDealerValue >= BUSTNUMBER)
            {
                DealerBust();
            }
        }
    }
    void Bust()
    {
        Debug.Log("PLAYER BUST!! Card value: " + currentPlayerValue.ToString());
        didBust = true;
    }
    void DealerBust()
    {
        Debug.Log("DEALER BUST!! Card value: " + currentDealerValue.ToString());
    }
    void UpdateValues()
    {
        string textValue = "Dealer: " + currentDealerValue.ToString() + "\nPlayer: " + currentPlayerValue.ToString();
        scoreText.text = textValue;
    }

    void Hit()
    {
        if (didStand) return;
        if (didBust) return;
        // IF CAN HIT
        CreateNewCard(true);
    }
    void Stand()
    {
        if (didStand) return;
        if (didBust) return;
        didStand = true;


        // now dealer plays

        //
        // if ()
        // {

        // }
        // bust condition
        while ((currentDealerValue < 21) && (currentDealerValue < currentPlayerValue))
        {
            CreateNewCard(false);
        }
        if (currentDealerValue == currentPlayerValue)
        {
            Debug.Log("Draw");
        }
        else if (currentDealerValue > 21)
        {
            Debug.Log("Dealer Bust");
        }
        else
        {
            Debug.Log("Dealer Win");
        }
    }
}
