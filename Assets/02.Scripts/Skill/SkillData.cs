using UnityEngine;

public enum SkillType
{
    Melee,
    Projectile
}

[CreateAssetMenu(fileName = "NewSkillData", menuName = "DungeonRush/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("기본")]
    public string skillName;
    public SkillType skillType;

    [Header("전투")]
    public float damage = 10f;
    public float range = 1.5f;
    public float cooldown = 1f;

    [Header("투사체 (Projectile 타입만)")]
    public Projectile projectilePrefab;
    public float projectileSpeed = 8f;
}