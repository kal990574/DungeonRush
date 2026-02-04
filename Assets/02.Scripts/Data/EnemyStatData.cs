using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStat", menuName = "DungeonRush/Enemy Stat Data")]
public class EnemyStatData : ScriptableObject
{
    [Header("체력")]
    public float maxHp = 50f;

    [Header("이동")]
    public float moveSpeed = 2f;

    [Header("공격")]
    public float attackDamage = 5f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
}