using System;

namespace DungeonRush.Stats.Data
{
    [Serializable]
    public class SkillPassiveData
    {
        public string EffectType;
        public string TargetStat;
        public string ModifyType;
        public float ModifyValue;
        public string ApplyScope;
        public string ApplySkillTag;
        public string ApplySkillId;
        public string TriggerType;
        public float TriggerChance;
        public float TriggerCoolTime;
        public float BuffDuration;
        public int StackLimit;
        public string PassivePrefabPath;

        public static readonly string[] AllParams =
        {
            "ModifyValue", "TriggerChance", "TriggerCoolTime",
            "BuffDuration", "StackLimit"
        };

        public float GetParam(string param)
        {
            return param switch
            {
                "ModifyValue" => ModifyValue,
                "TriggerChance" => TriggerChance,
                "TriggerCoolTime" => TriggerCoolTime,
                "BuffDuration" => BuffDuration,
                "StackLimit" => StackLimit,
                _ => throw new ArgumentException($"Unknown passive param: {param}")
            };
        }

        public void SetParam(string param, float value)
        {
            switch (param)
            {
                case "ModifyValue": ModifyValue = value; break;
                case "TriggerChance": TriggerChance = value; break;
                case "TriggerCoolTime": TriggerCoolTime = value; break;
                case "BuffDuration": BuffDuration = value; break;
                case "StackLimit": StackLimit = (int)value; break;
                default: throw new ArgumentException($"Unknown passive param: {param}");
            }
        }

        public SkillPassiveData Clone()
        {
            return (SkillPassiveData)MemberwiseClone();
        }
    }
}
