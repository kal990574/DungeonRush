using System;
using System.Collections.Generic;

namespace DungeonRush.Stats.Config
{
    public static class StatConfig
    {
        public const string CharacterCsvPath = "Data/Characters";
        public const string SkillCsvPath = "Data/Skills";
        public const string SkillLevelUpCsvPath = "Data/SkillLevelUps";

        private static readonly Dictionary<string, (float Min, float Max)> s_statCaps = new()
        {
            { "MaxHp", (1f, 99999f) },
            { "HpRegen", (0f, 9999f) },
            { "Armor", (0f, 9999f) },
            { "MoveSpeed", (0.1f, 50f) },
            { "ATK", (0f, 99999f) },
            { "AttackSpeed", (0.01f, 10f) },
            { "AttackRange", (0.1f, 100f) },
            { "CritChance", (0f, 1f) },
            { "CritDamage", (0f, 10f) },
            { "CDR", (0f, 0.8f) },
            { "AreaSize", (0.1f, 10f) },
            { "ProjectileCount", (0f, 100f) },
            { "ProjectileSpeed", (0f, 200f) },
            { "DetectRange", (0.1f, 200f) },
            { "Luck", (0f, 10f) }
        };

        public static float ClampStat(string key, float value)
        {
            if (s_statCaps.TryGetValue(key, out var cap))
            {
                return Math.Clamp(value, cap.Min, cap.Max);
            }

            return value;
        }

        public static (float Min, float Max) GetCap(string key)
        {
            if (s_statCaps.TryGetValue(key, out var cap))
            {
                return cap;
            }

            return (float.MinValue, float.MaxValue);
        }
    }
}
