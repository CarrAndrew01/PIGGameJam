using System.Collections.Generic;
using UnityEngine;

public class BidScript : MonoBehaviour
{
    // need to plan how much cash = a token and integrate taking in and out cash
    public int bidTokens = 0;
    public int maxBid = 5;

    public GameObject tokenPrefab;
    public List<GameObject> tokens;
    public Transform bidSpawnTransform;

    public void AddToBid()
    {
        if (bidTokens < 5)
        {
            GameObject newToken = Instantiate(tokenPrefab, transform);
            if (bidSpawnTransform != null)
            {
                newToken.transform.position = bidSpawnTransform.position;
            }
            tokens.Add(newToken);
            bidTokens++;
        }
    }
    public void RemoveFromBid()
    {
        if (bidTokens > 0)
        {
            // Destroy(tokens[bidTokens - 1]);
            // tokens.Remove(tokens[bidTokens - 1]);

            // bidTokens--;
            Destroy(tokens[0]);
            tokens.Remove(tokens[0]);

            bidTokens--;

        }
    }
}
