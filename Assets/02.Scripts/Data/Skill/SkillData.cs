using _02.Scripts.Core;
using UnityEngine;

namespace _02.Scripts.Data.Skill
{
    [CreateAssetMenu(fileName = "SK_New", menuName = "DungeonRush/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Basic Info")]
        public string skillName;
        public Sprite icon;

        [Header("Damage")]
        public float baseDamage = 10f;
        public DamageType damageType = DamageType.Physical;

        [Header("Targeting")]
        public float range = 5f;

        [Header("Cooldown")]
        public float cooldown = 2f;

        [Header("Scaling")]
        public float damagePerLevel = 0.1f;
        public int maxLevel = 5;
    }
}