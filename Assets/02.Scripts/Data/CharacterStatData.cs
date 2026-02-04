using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStat", menuName = "DungeonRush/Character Stat Data")]
public class CharacterStatData : ScriptableObject
{
    [Header("체력")]
    public float maxHp = 100f;

    [Header("공격")]
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("이동")]
    public float moveSpeed = 3f;

    [Header("탐지")]
    public float detectRange = 10f;
}