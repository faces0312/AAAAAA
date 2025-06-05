using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackResolver
{
    public int ResolveDamage(List<bool> coinResults, int baseAttackPower)
    {
        int heads = coinResults.Count(result => result);
        return baseAttackPower + (heads * 2);  // 앞면이 나올 때마다 공격력 2씩 증가
    }
}
