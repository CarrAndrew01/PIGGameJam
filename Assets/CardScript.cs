using System.Collections.Generic;
using TMPro;
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
    }
    public int GetCardValue()
    {
        // everything after 10 is counted as 10. aces are calculated in blackjack script, return as 1
        if (num >= 10) return 10;
        // return 1-9
        return num;
    }

}
