using System;
using System.Collections.Generic;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class SkillPassiveData
    {
        private readonly Dictionary<string, float> _params = new();

        public static readonly string[] AllParams =
        {
            "ModifyValue", "TriggerChance", "TriggerCoolTime",
            "BuffDuration", "StackLimit"
        };

        public string PassiveEffectType;
        public string TargetStat;
        public string ModifyType;
        public float ModifyValue { get => GetParam("ModifyValue"); set => SetParam("ModifyValue", value); }
        public string ApplyScope;
        public string ApplySkillTag;
        public string ApplySkillID;
        public string TriggerType;
        public float TriggerChance { get => GetParam("TriggerChance"); set => SetParam("TriggerChance", value); }
        public float TriggerCoolTime { get => GetParam("TriggerCoolTime"); set => SetParam("TriggerCoolTime", value); }
        public float BuffDuration { get => GetParam("BuffDuration"); set => SetParam("BuffDuration", value); }
        public int StackLimit { get => (int)GetParam("StackLimit"); set => SetParam("StackLimit", value); }
        public string PassivePrefabPath;

        public float GetParam(string param)
        {
            return _params.TryGetValue(param, out float v) ? v : 0f;
        }

        public void SetParam(string param, float value)
        {
            _params[param] = value;
        }

        public SkillPassiveData Clone()
        {
            var clone = new SkillPassiveData();
            foreach (var kvp in _params)
            {
                clone._params[kvp.Key] = kvp.Value;
            }

            clone.PassiveEffectType = PassiveEffectType;
            clone.TargetStat = TargetStat;
            clone.ModifyType = ModifyType;
            clone.ApplyScope = ApplyScope;
            clone.ApplySkillTag = ApplySkillTag;
            clone.ApplySkillID = ApplySkillID;
            clone.TriggerType = TriggerType;
            clone.PassivePrefabPath = PassivePrefabPath;
            return clone;
        }
    }
}
