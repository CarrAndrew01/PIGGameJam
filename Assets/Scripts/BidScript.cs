using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BidScript : MonoBehaviour
{
    // need to plan how much cash = a token and integrate taking in and out cash
    public static int bidTokens = 0;
    public static float bidTokenValue = 5f;

    [HideInInspector]
    public int maxBid = 5;

    public float Wager => bidTokens * bidTokenValue;


    public GameObject tokenPrefab;
    public List<GameObject> tokens;
    public Transform bidSpawnTransform;

    public delegate void WagerComplete();
    public static event WagerComplete wagerComplete;

    public TextMeshProUGUI playerTotalCash;
    public TextMeshProUGUI playerWager;


    private void OnEnable()
    {
        BlackjackScript.resetEvent += ResetValues;
    }
    private void OnDisable()
    {
        BlackjackScript.resetEvent -= ResetValues;
    }

    private void OnDestroy()
    {
        bidTokens = 0;
    }


    private void Start()
    {
        bidTokenValue = GameManager.GetPlayerStat(StatType.blackjackWager);
        ResetTextValues();
    }
    void ResetTextValues()
    {
        playerTotalCash.text = $"CASH - ${GameManager.Money:f2}";
        playerWager.text = $"WAGER - ${Wager:f2}";
    }
    public void AddToBid()
    {
        if (GameManager.Money >= bidTokenValue)
        {
            if (bidTokens < maxBid)
            {
                GameManager.AdjustMoney(-bidTokenValue);
                GameObject newToken = Instantiate(tokenPrefab, transform);
                if (bidSpawnTransform != null)
                {
                    newToken.transform.position = bidSpawnTransform.position;
                }
                tokens.Add(newToken);
                bidTokens++;
                ResetTextValues();
            }
        }
    }
    public void RemoveFromBid()
    {
        if (bidTokens > 0)
        {
            Destroy(tokens[0]);
            tokens.Remove(tokens[0]);

            bidTokens--;
            GameManager.AdjustMoney(bidTokenValue);
            ResetTextValues();
        }
    }
    public void BeginMatch()
    {
        // calls match start on blackjack script
        if (bidTokens > 0)
        {
            wagerComplete?.Invoke();
        }
    }
    void ResetValues()
    {
        bidTokens = 0;
        foreach (var token in tokens)
        {
            Destroy(token);
        }
        tokens.Clear();
        ResetTextValues();
    }

}
