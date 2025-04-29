using System.Collections;
using UnityEngine;

public class BattleManager : SingletonWithMono<BattleManager>, IBaseManager
{
    public bool IsInitialized { get; set; }

    private Player _player;
    private Enemy _enemy;


    public void Init()
    {
    }

    public void SetupPlayer(Player player)
    {
        _player = player;
    }
    public void SetupEnemy(Enemy enemy)
    {
        _enemy = enemy;
    }

    public void StartBattle()
    {
        Debug.Log("���� ����!");
        StartCoroutine(DoBattleTurn());  // ������� �ڷ�ƾ����!
    }

    private IEnumerator DoBattleTurn()
    {
        int enemyFixedDamage = _enemy.GetFixedDamage();

        yield return _player.StartTurn(
            enemyFixedDamage,
            (playerDamage) => _enemy.TakeDamage(playerDamage)
        );
    }
}
