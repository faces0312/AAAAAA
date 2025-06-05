using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int Hp = 100;
    [SerializeField] private int fixedDamage = 15;

    void Start()
    {
        BattleManager.Instance.SetupEnemy(this);
    }
    public int GetFixedDamage()
    {
        return fixedDamage;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log($"적이 받은 데미지: {damage}, 남은 적 체력 HP: {Hp}");
    }
}
