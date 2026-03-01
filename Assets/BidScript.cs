using System.Collections.Generic;
using UnityEngine;

public class BidScript : MonoBehaviour
{
    // need to plan how much cash = a token and integrate taking in and out cash
    public static int bidTokens = 0;
    [HideInInspector]
    public static float bidTokenValue = 5;
    public int maxBid = 5;

    public GameObject tokenPrefab;
    public List<GameObject> tokens;
    public Transform bidSpawnTransform;

    public delegate void WagerComplete();
    public static event WagerComplete wagerComplete;


    public void AddToBid()
    {
        if (GameManager.Money >= 5)
        {
            if (bidTokens < 5)
            {
                GameManager.AdjustMoney(-5);
                GameObject newToken = Instantiate(tokenPrefab, transform);
                if (bidSpawnTransform != null)
                {
                    newToken.transform.position = bidSpawnTransform.position;
                }
                tokens.Add(newToken);
                bidTokens++;
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
            GameManager.AdjustMoney(5);
        }
    }
    public void BeginMatch()
    {
        // calls match start on blackjack script
        if (bidTokens > 0)
        {
            Debug.Log("HELLO");
            wagerComplete?.Invoke();
        }
    }

}
