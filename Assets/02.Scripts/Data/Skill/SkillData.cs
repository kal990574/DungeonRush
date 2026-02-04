using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Data.Skill
{
    [CreateAssetMenu(fileName = "SK_New", menuName = "DungeonRush/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Basic Info")]
        public string SkillName;
        public Sprite Icon;

        [Header("Damage")]
        public float BaseDamage = 10f;
        public DamageType DamageType = DamageType.Physical;

        [Header("Targeting")]
        public float Range = 5f;

        [Header("Cooldown")]
        public float Cooldown = 2f;

        [Header("Scaling")]
        public float DamagePerLevel = 0.1f;
        public int MaxLevel = 5;
    }
}