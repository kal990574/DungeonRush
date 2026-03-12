using System;
using System.Collections.Generic;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class BaseStats
    {
        private readonly Dictionary<string, float> _values = new();

        public static readonly string[] AllKeys =
        {
            "ATK", "CDR", "CritRate", "CritDamage",
            "HP", "HPRegen", "Armor", "Evasion",
            "MoveSpeed", "PickupRange", "ExpGain", "GoldGain", "Luck",
            "Lifesteal", "Thorns"
        };

        // 공격.
        public float ATK { get => GetValue("ATK"); set => SetValue("ATK", value); }
        public float CDR { get => GetValue("CDR"); set => SetValue("CDR", value); }
        public float CritRate { get => GetValue("CritRate"); set => SetValue("CritRate", value); }
        public float CritDamage { get => GetValue("CritDamage"); set => SetValue("CritDamage", value); }

        // 방어.
        public float HP { get => GetValue("HP"); set => SetValue("HP", value); }
        public float HPRegen { get => GetValue("HPRegen"); set => SetValue("HPRegen", value); }
        public float Armor { get => GetValue("Armor"); set => SetValue("Armor", value); }
        public float Evasion { get => GetValue("Evasion"); set => SetValue("Evasion", value); }

        // 유틸/파밍.
        public float MoveSpeed { get => GetValue("MoveSpeed"); set => SetValue("MoveSpeed", value); }
        public float PickupRange { get => GetValue("PickupRange"); set => SetValue("PickupRange", value); }
        public float ExpGain { get => GetValue("ExpGain"); set => SetValue("ExpGain", value); }
        public float GoldGain { get => GetValue("GoldGain"); set => SetValue("GoldGain", value); }
        public float Luck { get => GetValue("Luck"); set => SetValue("Luck", value); }

        // 특수.
        public float Lifesteal { get => GetValue("Lifesteal"); set => SetValue("Lifesteal", value); }
        public float Thorns { get => GetValue("Thorns"); set => SetValue("Thorns", value); }

        public float GetValue(string key)
        {
            return _values.TryGetValue(key, out float v) ? v : 0f;
        }

        public void SetValue(string key, float value)
        {
            _values[key] = value;
        }

        public bool HasKey(string key)
        {
            return Array.IndexOf(AllKeys, key) >= 0;
        }

        public BaseStats Clone()
        {
            var clone = new BaseStats();
            foreach (var kvp in _values)
            {
                clone._values[kvp.Key] = kvp.Value;
            }

            return clone;
        }
    }
}
