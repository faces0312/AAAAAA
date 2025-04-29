using UnityEngine;

public class DamageComparer
{
    public bool IsPlayerStronger(int playerDamage, int enemyDamage)
    {
        return playerDamage >= enemyDamage;
    }
}