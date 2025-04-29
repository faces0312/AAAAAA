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
        Debug.Log("�÷��̾� �� ����");

        var coinResults = _coinSystem.FlipCoins(3);
        int playerDamage = _attackResolver.ResolveDamage(coinResults);

        Debug.Log($"�÷��̾� ������: {playerDamage}, �� ���� ������: {enemyDamage}");

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
        Debug.Log($"�÷��̾� �ǰ�! ������: {damage}, ���� HP: {Hp}");
    }
}
