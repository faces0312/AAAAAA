using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int Hp = 100;
    [SerializeField] private int fixedDamage = 15;

    public int GetFixedDamage()
    {
        return fixedDamage;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Debug.Log($"적 피격! 데미지: {damage}, 남은 HP: {Hp}");
    }
}
