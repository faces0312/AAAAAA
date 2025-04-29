using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackResolver
{
    public int ResolveDamage(List<bool> coinResults)
    {
        int heads = coinResults.Count(result => result);
        return heads switch
        {
            0 => 0,   // ½ÇÆÐ
            1 => 10,
            2 => 20,
            3 => 30,
            _ => 0
        };
    }
}
