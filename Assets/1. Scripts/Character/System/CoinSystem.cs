using System.Collections.Generic;
using UnityEngine;

public class CoinSystem
{
    public List<bool> FlipCoins(int count)
    {
        var results = new List<bool>();
        for (int i = 0; i < count; i++)
        {
            results.Add(Random.value > 0.5f); // true = ¾Õ¸é
        }
        return results;
    }
}