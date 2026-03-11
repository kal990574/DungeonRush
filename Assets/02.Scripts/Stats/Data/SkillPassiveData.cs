using System;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class SkillPassiveData
    {
        public string TargetStat;
        public float FlatBonus;
        public float PercentBonus;
        public float Duration;
        public int MaxStacks;
        public float TriggerChance;
        public float TriggerCooldown;
        public float EffectRadius;
        public float TickInterval;
        public float TickValue;
        public float ShieldAmount;
        public float DamageReduction;
        public float LifeSteal;

        public static readonly string[] AllParams =
        {
            "TargetStat", "FlatBonus", "PercentBonus", "Duration", "MaxStacks",
            "TriggerChance", "TriggerCooldown", "EffectRadius", "TickInterval",
            "TickValue", "ShieldAmount", "DamageReduction", "LifeSteal"
        };

        public float GetParam(string param)
        {
            return param switch
            {
                "FlatBonus" => FlatBonus,
                "PercentBonus" => PercentBonus,
                "Duration" => Duration,
                "MaxStacks" => MaxStacks,
                "TriggerChance" => TriggerChance,
                "TriggerCooldown" => TriggerCooldown,
                "EffectRadius" => EffectRadius,
                "TickInterval" => TickInterval,
                "TickValue" => TickValue,
                "ShieldAmount" => ShieldAmount,
                "DamageReduction" => DamageReduction,
                "LifeSteal" => LifeSteal,
                _ => throw new ArgumentException($"Unknown passive param: {param}")
            };
        }

        public void SetParam(string param, float value)
        {
            switch (param)
            {
                case "FlatBonus": FlatBonus = value; break;
                case "PercentBonus": PercentBonus = value; break;
                case "Duration": Duration = value; break;
                case "MaxStacks": MaxStacks = (int)value; break;
                case "TriggerChance": TriggerChance = value; break;
                case "TriggerCooldown": TriggerCooldown = value; break;
                case "EffectRadius": EffectRadius = value; break;
                case "TickInterval": TickInterval = value; break;
                case "TickValue": TickValue = value; break;
                case "ShieldAmount": ShieldAmount = value; break;
                case "DamageReduction": DamageReduction = value; break;
                case "LifeSteal": LifeSteal = value; break;
                default: throw new ArgumentException($"Unknown passive param: {param}");
            }
        }

        public SkillPassiveData Clone()
        {
            return (SkillPassiveData)MemberwiseClone();
        }
    }
}
