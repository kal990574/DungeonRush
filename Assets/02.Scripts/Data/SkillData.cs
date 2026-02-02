using UnityEngine;
using DungeonRush.Core;

namespace DungeonRush.Data
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "DungeonRush/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Basic Info")]
        public string skillName;
        public SkillType skillType;
        public Sprite icon;

        [Header("Stats")]
        public float damage;
        public float cooldown;

        [Header("Description")]
        [TextArea(2, 4)]
        public string description;
    }
}
