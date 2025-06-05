using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public int Hp = 100;
    [SerializeField] int BaseAttackPower = 1;

    private CoinSystem _coinSystem = new CoinSystem();
    private AttackResolver _attackResolver = new AttackResolver();
    private DamageComparer _damageComparer = new DamageComparer();

    void Start()
    {
        BattleManager.Instance.SetupPlayer(this);
    }

    public IEnumerator StartTurn(int enemyDamage, Action<int> onPlayerWins)
    {
        var coinResults = _coinSystem.FlipCoins(3);
        Debug.Log("Coin Results: " + string.Join(", ", coinResults)); // true/false 리스트 확인
        int playerDamage = _attackResolver.ResolveDamage(coinResults, BaseAttackPower);
        
        
        Debug.Log($"Player Damage: {playerDamage}");

        // DiceManager의 주사위 결과를 데미지에 추가
        playerDamage += DiceManager.Instance._diceResult;

        Debug.Log($"Player Damage: {playerDamage}");

        if (_damageComparer.IsPlayerStronger(playerDamage, enemyDamage))
        {
            Debug.Log("Player Wins");
            onPlayerWins?.Invoke(playerDamage);
        }
        else
        {
            Debug.Log("Enemy Wins");
            TakeDamage(enemyDamage);
        }

        yield return null;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log($"내가 받은 데미지: {damage}, 남은 나의 체력 HP: {Hp}");
    }
}
