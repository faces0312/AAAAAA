using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public int Hp = 100;

    private CoinSystem _coinSystem = new CoinSystem();
    private AttackResolver _attackResolver = new AttackResolver();
    private DamageComparer _damageComparer = new DamageComparer();

    public IEnumerator StartTurn(int enemyDamage, Action<int> onPlayerWins)
    {
        Debug.Log("플레이어 턴 시작");

        var coinResults = _coinSystem.FlipCoins(3);
        int playerDamage = _attackResolver.ResolveDamage(coinResults);

        Debug.Log($"플레이어 데미지: {playerDamage}, 적 고정 데미지: {enemyDamage}");

        if (_damageComparer.IsPlayerStronger(playerDamage, enemyDamage))
        {
            onPlayerWins?.Invoke(playerDamage);
        }
        else
        {
            TakeDamage(enemyDamage);
        }

        yield return null;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log($"플레이어 피격! 데미지: {damage}, 남은 HP: {Hp}");
    }
}
