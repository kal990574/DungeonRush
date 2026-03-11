using System;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class SkillAttackData
    {
        public float Damage;
        public float DamageCoeff;
        public float Cooldown;
        public float Range;
        public float ProjectileSpeed;
        public int ProjectileCount;
        public int Pierce;
        public float AreaSize;
        public float Duration;
        public float TickInterval;
        public float TickDamageCoeff;
        public int BounceCount;
        public float KnockbackForce;
        public float CritChanceBonus;
        public float CritDamageBonus;
        public int ChainCount;
        public float ChainDamageDecay;
        public float CastDelay;

        public static readonly string[] AllParams =
        {
            "Damage", "DamageCoeff", "Cooldown", "Range", "ProjectileSpeed",
            "ProjectileCount", "Pierce", "AreaSize", "Duration", "TickInterval",
            "TickDamageCoeff", "BounceCount", "KnockbackForce", "CritChanceBonus",
            "CritDamageBonus", "ChainCount", "ChainDamageDecay", "CastDelay"
        };

        public float GetParam(string param)
        {
            return param switch
            {
                "Damage" => Damage,
                "DamageCoeff" => DamageCoeff,
                "Cooldown" => Cooldown,
                "Range" => Range,
                "ProjectileSpeed" => ProjectileSpeed,
                "ProjectileCount" => ProjectileCount,
                "Pierce" => Pierce,
                "AreaSize" => AreaSize,
                "Duration" => Duration,
                "TickInterval" => TickInterval,
                "TickDamageCoeff" => TickDamageCoeff,
                "BounceCount" => BounceCount,
                "KnockbackForce" => KnockbackForce,
                "CritChanceBonus" => CritChanceBonus,
                "CritDamageBonus" => CritDamageBonus,
                "ChainCount" => ChainCount,
                "ChainDamageDecay" => ChainDamageDecay,
                "CastDelay" => CastDelay,
                _ => throw new ArgumentException($"Unknown attack param: {param}")
            };
        }

        public void SetParam(string param, float value)
        {
            switch (param)
            {
                case "Damage": Damage = value; break;
                case "DamageCoeff": DamageCoeff = value; break;
                case "Cooldown": Cooldown = value; break;
                case "Range": Range = value; break;
                case "ProjectileSpeed": ProjectileSpeed = value; break;
                case "ProjectileCount": ProjectileCount = (int)value; break;
                case "Pierce": Pierce = (int)value; break;
                case "AreaSize": AreaSize = value; break;
                case "Duration": Duration = value; break;
                case "TickInterval": TickInterval = value; break;
                case "TickDamageCoeff": TickDamageCoeff = value; break;
                case "BounceCount": BounceCount = (int)value; break;
                case "KnockbackForce": KnockbackForce = value; break;
                case "CritChanceBonus": CritChanceBonus = value; break;
                case "CritDamageBonus": CritDamageBonus = value; break;
                case "ChainCount": ChainCount = (int)value; break;
                case "ChainDamageDecay": ChainDamageDecay = value; break;
                case "CastDelay": CastDelay = value; break;
                default: throw new ArgumentException($"Unknown attack param: {param}");
            }
        }

        public SkillAttackData Clone()
        {
            return (SkillAttackData)MemberwiseClone();
        }
    }
}
