using System;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class BaseStats
    {
        // 공격.
        public float ATK;
        public float CDR;
        public float CritRate;
        public float CritDamage;

        // 방어.
        public float HP;
        public float HPRegen;
        public float Armor;
        public float Evasion;

        // 유틸/파밍.
        public float MoveSpeed;
        public float PickupRange;
        public float ExpGain;
        public float GoldGain;
        public float Luck;

        // 특수.
        public float Lifesteal;
        public float Thorns;

        public static readonly string[] AllKeys =
        {
            "ATK", "CDR", "CritRate", "CritDamage",
            "HP", "HPRegen", "Armor", "Evasion",
            "MoveSpeed", "PickupRange", "ExpGain", "GoldGain", "Luck",
            "Lifesteal", "Thorns"
        };

        public float GetValue(string key)
        {
            return key switch
            {
                "ATK" => ATK,
                "CDR" => CDR,
                "CritRate" => CritRate,
                "CritDamage" => CritDamage,
                "HP" => HP,
                "HPRegen" => HPRegen,
                "Armor" => Armor,
                "Evasion" => Evasion,
                "MoveSpeed" => MoveSpeed,
                "PickupRange" => PickupRange,
                "ExpGain" => ExpGain,
                "GoldGain" => GoldGain,
                "Luck" => Luck,
                "Lifesteal" => Lifesteal,
                "Thorns" => Thorns,
                _ => throw new ArgumentException($"Unknown stat key: {key}")
            };
        }

        public void SetValue(string key, float value)
        {
            switch (key)
            {
                case "ATK": ATK = value; break;
                case "CDR": CDR = value; break;
                case "CritRate": CritRate = value; break;
                case "CritDamage": CritDamage = value; break;
                case "HP": HP = value; break;
                case "HPRegen": HPRegen = value; break;
                case "Armor": Armor = value; break;
                case "Evasion": Evasion = value; break;
                case "MoveSpeed": MoveSpeed = value; break;
                case "PickupRange": PickupRange = value; break;
                case "ExpGain": ExpGain = value; break;
                case "GoldGain": GoldGain = value; break;
                case "Luck": Luck = value; break;
                case "Lifesteal": Lifesteal = value; break;
                case "Thorns": Thorns = value; break;
                default: throw new ArgumentException($"Unknown stat key: {key}");
            }
        }

        public BaseStats Clone()
        {
            return (BaseStats)MemberwiseClone();
        }
    }
}
