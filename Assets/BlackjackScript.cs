using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlackjackScript : MonoBehaviour
{
    const int CARDNUMMIN = 1;
    const int CARDNUMMAX = 9;
    const int BUSTNUMBER = 21;
    public int currentPlayerValue = 0;
    String[] SUITS = new string[] { "♥", "♦", "♣", "♠" };

    public GameObject cardPrefab;
    public Transform canvas;

    public List<GameObject> playerCards = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cardPrefab != null && canvas != null)
        {
            CreateNewCard();
            CreateNewCard();
            CreateNewCard();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CreateNewCard()
    {
        int cardNumber = Random.Range(CARDNUMMIN, CARDNUMMAX);
        string newCardValue = cardNumber.ToString();
        int newCardSuitIndex = Random.Range(0, 3);
        string newCardSuit = SUITS[newCardSuitIndex];

        GameObject newCard = Instantiate(cardPrefab, canvas);

        newCard.GetComponentInChildren<TextMeshProUGUI>().text = newCardValue + newCardSuit;

        currentPlayerValue += cardNumber;
        playerCards.Add(newCard);

        if (currentPlayerValue > BUSTNUMBER)
        {
            Bust();
        }
    }
    void Bust()
    {

    }
}
