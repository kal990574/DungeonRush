using System;
using System.Collections.Generic;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class SkillAttackData
    {
        private readonly Dictionary<string, float> _params = new();

        public static readonly string[] AllParams =
        {
            "SkillCoef", "BaseCoolTime", "BaseRange", "BaseRadius",
            "BaseKnockback", "PierceCount", "ChainCount", "BounceCount",
            "BaseProj", "BaseProjSpeed", "BaseDuration", "TickInterval",
            "TickCoef", "MaxStack", "StackRule"
        };

        public float SkillCoef { get => GetParam("SkillCoef"); set => SetParam("SkillCoef", value); }
        public float BaseCoolTime { get => GetParam("BaseCoolTime"); set => SetParam("BaseCoolTime", value); }
        public bool CritEnabled;
        public bool LifestealEnabled;
        public float BaseRange { get => GetParam("BaseRange"); set => SetParam("BaseRange", value); }
        public float BaseRadius { get => GetParam("BaseRadius"); set => SetParam("BaseRadius", value); }
        public float BaseKnockback { get => GetParam("BaseKnockback"); set => SetParam("BaseKnockback", value); }
        public int PierceCount { get => (int)GetParam("PierceCount"); set => SetParam("PierceCount", value); }
        public int ChainCount { get => (int)GetParam("ChainCount"); set => SetParam("ChainCount", value); }
        public int BounceCount { get => (int)GetParam("BounceCount"); set => SetParam("BounceCount", value); }
        public int BaseProj { get => (int)GetParam("BaseProj"); set => SetParam("BaseProj", value); }
        public float BaseProjSpeed { get => GetParam("BaseProjSpeed"); set => SetParam("BaseProjSpeed", value); }
        public string FirePattern;
        public float BaseDuration { get => GetParam("BaseDuration"); set => SetParam("BaseDuration", value); }
        public float TickInterval { get => GetParam("TickInterval"); set => SetParam("TickInterval", value); }
        public float TickCoef { get => GetParam("TickCoef"); set => SetParam("TickCoef", value); }
        public int MaxStack { get => (int)GetParam("MaxStack"); set => SetParam("MaxStack", value); }
        public int StackRule { get => (int)GetParam("StackRule"); set => SetParam("StackRule", value); }

        public float GetParam(string param)
        {
            return _params.TryGetValue(param, out float v) ? v : 0f;
        }

        public void SetParam(string param, float value)
        {
            _params[param] = value;
        }

        public SkillAttackData Clone()
        {
            var clone = new SkillAttackData();
            foreach (var kvp in _params)
            {
                clone._params[kvp.Key] = kvp.Value;
            }

            clone.CritEnabled = CritEnabled;
            clone.LifestealEnabled = LifestealEnabled;
            clone.FirePattern = FirePattern;
            return clone;
        }
    }
}
