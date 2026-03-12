using System;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class SkillAttackData
    {
        public float SkillCoef;
        public float BaseCoolTime;
        public bool CritEnabled;
        public bool LifestealEnabled;
        public float BaseRange;
        public float BaseRadius;
        public float BaseKnockback;
        public int PierceCount;
        public int ChainCount;
        public int BounceCount;
        public int BaseProj;
        public float BaseProjSpeed;
        public string FirePattern;
        public float BaseDuration;
        public float TickInterval;
        public float TickCoef;
        public int MaxStack;
        public int StackRule;

        public static readonly string[] AllParams =
        {
            "SkillCoef", "BaseCoolTime", "BaseRange", "BaseRadius",
            "BaseKnockback", "PierceCount", "ChainCount", "BounceCount",
            "BaseProj", "BaseProjSpeed", "BaseDuration", "TickInterval",
            "TickCoef", "MaxStack", "StackRule"
        };

        public float GetParam(string param)
        {
            return param switch
            {
                "SkillCoef" => SkillCoef,
                "BaseCoolTime" => BaseCoolTime,
                "BaseRange" => BaseRange,
                "BaseRadius" => BaseRadius,
                "BaseKnockback" => BaseKnockback,
                "PierceCount" => PierceCount,
                "ChainCount" => ChainCount,
                "BounceCount" => BounceCount,
                "BaseProj" => BaseProj,
                "BaseProjSpeed" => BaseProjSpeed,
                "BaseDuration" => BaseDuration,
                "TickInterval" => TickInterval,
                "TickCoef" => TickCoef,
                "MaxStack" => MaxStack,
                "StackRule" => StackRule,
                _ => throw new ArgumentException($"Unknown attack param: {param}")
            };
        }

        public void SetParam(string param, float value)
        {
            switch (param)
            {
                case "SkillCoef": SkillCoef = value; break;
                case "BaseCoolTime": BaseCoolTime = value; break;
                case "BaseRange": BaseRange = value; break;
                case "BaseRadius": BaseRadius = value; break;
                case "BaseKnockback": BaseKnockback = value; break;
                case "PierceCount": PierceCount = (int)value; break;
                case "ChainCount": ChainCount = (int)value; break;
                case "BounceCount": BounceCount = (int)value; break;
                case "BaseProj": BaseProj = (int)value; break;
                case "BaseProjSpeed": BaseProjSpeed = value; break;
                case "BaseDuration": BaseDuration = value; break;
                case "TickInterval": TickInterval = value; break;
                case "TickCoef": TickCoef = value; break;
                case "MaxStack": MaxStack = (int)value; break;
                case "StackRule": StackRule = (int)value; break;
                default: throw new ArgumentException($"Unknown attack param: {param}");
            }
        }

        public SkillAttackData Clone()
        {
            var clone = (SkillAttackData)MemberwiseClone();
            clone.FirePattern = FirePattern;
            return clone;
        }
    }
}
