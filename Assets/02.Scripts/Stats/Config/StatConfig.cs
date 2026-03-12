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
            { "ATK", (0f, 99999f) },
            { "CDR", (0f, 0.8f) },
            { "CritRate", (0f, 1f) },
            { "CritDamage", (0f, 10f) },
            { "HP", (1f, 99999f) },
            { "HPRegen", (0f, 9999f) },
            { "Armor", (0f, 9999f) },
            { "Evasion", (0f, 1f) },
            { "MoveSpeed", (0.1f, 50f) },
            { "PickupRange", (0.1f, 100f) },
            { "ExpGain", (0f, 10f) },
            { "GoldGain", (0f, 10f) },
            { "Luck", (0f, 10f) },
            { "Lifesteal", (0f, 1f) },
            { "Thorns", (0f, 9999f) }
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
