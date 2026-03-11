using System;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class BaseStats
    {
        public float MaxHp;
        public float HpRegen;
        public float Armor;
        public float MoveSpeed;
        public float ATK;
        public float AttackSpeed;
        public float AttackRange;
        public float CritChance;
        public float CritDamage;
        public float CDR;
        public float AreaSize;
        public float ProjectileCount;
        public float ProjectileSpeed;
        public float DetectRange;
        public float Luck;

        public static readonly string[] AllKeys =
        {
            "MaxHp", "HpRegen", "Armor", "MoveSpeed", "ATK",
            "AttackSpeed", "AttackRange", "CritChance", "CritDamage",
            "CDR", "AreaSize", "ProjectileCount", "ProjectileSpeed",
            "DetectRange", "Luck"
        };

        public float GetValue(string key)
        {
            return key switch
            {
                "MaxHp" => MaxHp,
                "HpRegen" => HpRegen,
                "Armor" => Armor,
                "MoveSpeed" => MoveSpeed,
                "ATK" => ATK,
                "AttackSpeed" => AttackSpeed,
                "AttackRange" => AttackRange,
                "CritChance" => CritChance,
                "CritDamage" => CritDamage,
                "CDR" => CDR,
                "AreaSize" => AreaSize,
                "ProjectileCount" => ProjectileCount,
                "ProjectileSpeed" => ProjectileSpeed,
                "DetectRange" => DetectRange,
                "Luck" => Luck,
                _ => throw new ArgumentException($"Unknown stat key: {key}")
            };
        }

        public void SetValue(string key, float value)
        {
            switch (key)
            {
                case "MaxHp": MaxHp = value; break;
                case "HpRegen": HpRegen = value; break;
                case "Armor": Armor = value; break;
                case "MoveSpeed": MoveSpeed = value; break;
                case "ATK": ATK = value; break;
                case "AttackSpeed": AttackSpeed = value; break;
                case "AttackRange": AttackRange = value; break;
                case "CritChance": CritChance = value; break;
                case "CritDamage": CritDamage = value; break;
                case "CDR": CDR = value; break;
                case "AreaSize": AreaSize = value; break;
                case "ProjectileCount": ProjectileCount = value; break;
                case "ProjectileSpeed": ProjectileSpeed = value; break;
                case "DetectRange": DetectRange = value; break;
                case "Luck": Luck = value; break;
                default: throw new ArgumentException($"Unknown stat key: {key}");
            }
        }

        public BaseStats Clone()
        {
            return (BaseStats)MemberwiseClone();
        }
    }
}
