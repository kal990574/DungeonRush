using UnityEngine;
using DungeonRush.Core;

namespace DungeonRush.Data
{
    [CreateAssetMenu(fileName = "CardData", menuName = "DungeonRush/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Basic Info")]
        public string cardName;
        public CardRarity rarity;
        public CardEffectType effectType;
        public Sprite icon;

        [Header("Effect")]
        public StatType targetStat;
        public float effectValue;

        [Header("Skill")]
        public SkillData linkedSkill;

        [Header("Description")]
        [TextArea(2, 4)]
        public string description;
    }
}
