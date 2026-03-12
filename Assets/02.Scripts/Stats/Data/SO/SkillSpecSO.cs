using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonRush.Stats.Data.SO
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "DungeonRush/Stats/Skill Spec")]
    public class SkillSpecSO : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _id;
        [SerializeField] private string _skillName;
        [FormerlySerializedAs("_skillType")] [SerializeField] private ESkillType eSkillType;
        [SerializeField] private string _subType;
        [SerializeField] private int _maxLevel = 5;
        [SerializeField] private int _skillGroup;
        [SerializeField] private string _grade;
        [SerializeField] private float _rate;
        [SerializeField] private string[] _linkedEffectGroupIds;
        [SerializeField] private string _descKey;
        [SerializeField] private string _iconPath;
        [SerializeField] private string _prefabPath;
        [SerializeField] private string _castVFXPath;
        [SerializeField] private string _hitVFXPath;
        [SerializeField] private string _sfxPath;
        [SerializeField] private string _weaponTagReq;

        [Header("Attack Data (ActiveAttack only)")]
        [SerializeField] private SerializedAttackData _attackData;

        [Header("Passive Data (Passive only)")]
        [SerializeField] private SerializedPassiveData _passiveData;

        [Header("Level Up Data")]
        [SerializeField] private SerializedLevelUpEntry[] _levelUpEntries;

        public string Id => _id;

        public SkillSpec ToSkillSpec()
        {
            var spec = new SkillSpec
            {
                Id = _id,
                Name = _skillName,
                ESkillType = eSkillType,
                SubType = _subType,
                MaxLevel = _maxLevel,
                SkillGroup = _skillGroup,
                Grade = _grade,
                Rate = _rate,
                LinkedEffectGroupIds = _linkedEffectGroupIds ?? Array.Empty<string>(),
                DescKey = _descKey,
                IconPath = _iconPath,
                PrefabPath = _prefabPath,
                CastVFXPath = _castVFXPath,
                HitVFXPath = _hitVFXPath,
                SFXPath = _sfxPath,
                WeaponTagReq = _weaponTagReq
            };

            if (eSkillType == ESkillType.ActiveAttack)
            {
                spec.AttackData = _attackData.ToSkillAttackData();
            }
            else
            {
                spec.PassiveData = _passiveData.ToSkillPassiveData();
            }

            return spec;
        }

        public List<SkillLevelUpData> ToLevelUpDataList()
        {
            var result = new List<SkillLevelUpData>();
            if (_levelUpEntries == null || _levelUpEntries.Length == 0)
            {
                return result;
            }

            // 같은 레벨의 수정을 하나의 SkillLevelUpData에 통합.
            var levelMap = new Dictionary<int, SkillLevelUpData>();
            foreach (var entry in _levelUpEntries)
            {
                var mod = new ParamModification(
                    entry.ParamName, entry.ValueType, entry.ParamValue,
                    string.IsNullOrEmpty(entry.ConditionType) ? null : entry.ConditionType,
                    string.IsNullOrEmpty(entry.ConditionValue) ? null : entry.ConditionValue);

                if (!levelMap.TryGetValue(entry.SkillLevel, out var levelUpData))
                {
                    levelUpData = new SkillLevelUpData(
                        _id, entry.SkillLevel, new List<ParamModification>(),
                        string.IsNullOrEmpty(entry.UITextKey) ? null : entry.UITextKey);
                    levelMap[entry.SkillLevel] = levelUpData;
                }

                levelUpData.Modifications.Add(mod);
            }

            result.AddRange(levelMap.Values);
            result.Sort((a, b) => a.SkillLevel.CompareTo(b.SkillLevel));
            return result;
        }

#if UNITY_EDITOR
        public void SetFromSkillSpec(SkillSpec spec)
        {
            _id = spec.Id;
            _skillName = spec.Name;
            eSkillType = spec.ESkillType;
            _subType = spec.SubType;
            _maxLevel = spec.MaxLevel;
            _skillGroup = spec.SkillGroup;
            _grade = spec.Grade;
            _rate = spec.Rate;
            _linkedEffectGroupIds = spec.LinkedEffectGroupIds;
            _descKey = spec.DescKey;
            _iconPath = spec.IconPath;
            _prefabPath = spec.PrefabPath;
            _castVFXPath = spec.CastVFXPath;
            _hitVFXPath = spec.HitVFXPath;
            _sfxPath = spec.SFXPath;
            _weaponTagReq = spec.WeaponTagReq;

            if (spec.IsAttack && spec.AttackData != null)
            {
                _attackData = SerializedAttackData.FromSkillAttackData(spec.AttackData);
            }
            else
            {
                _attackData = new SerializedAttackData();
            }

            if (spec.IsPassive && spec.PassiveData != null)
            {
                _passiveData = SerializedPassiveData.FromSkillPassiveData(spec.PassiveData);
            }
            else
            {
                _passiveData = new SerializedPassiveData();
            }
        }

        public void SetLevelUpEntries(List<SkillLevelUpData> levelUps)
        {
            var entries = new List<SerializedLevelUpEntry>();
            foreach (var levelUp in levelUps)
            {
                foreach (var mod in levelUp.Modifications)
                {
                    entries.Add(new SerializedLevelUpEntry
                    {
                        SkillLevel = levelUp.SkillLevel,
                        ParamName = mod.ParamName,
                        ValueType = mod.ValueType,
                        ParamValue = mod.ParamValue,
                        ConditionType = mod.ConditionType,
                        ConditionValue = mod.ConditionValue,
                        UITextKey = levelUp.UITextKey
                    });
                }
            }

            _levelUpEntries = entries.ToArray();
        }
#endif

        [Serializable]
        public struct SerializedAttackData
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

            public SkillAttackData ToSkillAttackData()
            {
                var data = new SkillAttackData();
                data.SkillCoef = SkillCoef;
                data.BaseCoolTime = BaseCoolTime;
                data.CritEnabled = CritEnabled;
                data.LifestealEnabled = LifestealEnabled;
                data.BaseRange = BaseRange;
                data.BaseRadius = BaseRadius;
                data.BaseKnockback = BaseKnockback;
                data.PierceCount = PierceCount;
                data.ChainCount = ChainCount;
                data.BounceCount = BounceCount;
                data.BaseProj = BaseProj;
                data.BaseProjSpeed = BaseProjSpeed;
                data.FirePattern = FirePattern;
                data.BaseDuration = BaseDuration;
                data.TickInterval = TickInterval;
                data.TickCoef = TickCoef;
                data.MaxStack = MaxStack;
                data.StackRule = StackRule;
                return data;
            }

            public static SerializedAttackData FromSkillAttackData(SkillAttackData data)
            {
                return new SerializedAttackData
                {
                    SkillCoef = data.SkillCoef,
                    BaseCoolTime = data.BaseCoolTime,
                    CritEnabled = data.CritEnabled,
                    LifestealEnabled = data.LifestealEnabled,
                    BaseRange = data.BaseRange,
                    BaseRadius = data.BaseRadius,
                    BaseKnockback = data.BaseKnockback,
                    PierceCount = data.PierceCount,
                    ChainCount = data.ChainCount,
                    BounceCount = data.BounceCount,
                    BaseProj = data.BaseProj,
                    BaseProjSpeed = data.BaseProjSpeed,
                    FirePattern = data.FirePattern,
                    BaseDuration = data.BaseDuration,
                    TickInterval = data.TickInterval,
                    TickCoef = data.TickCoef,
                    MaxStack = data.MaxStack,
                    StackRule = data.StackRule
                };
            }
        }

        [Serializable]
        public struct SerializedPassiveData
        {
            public string PassiveEffectType;
            public string TargetStat;
            public string ModifyType;
            public float ModifyValue;
            public string ApplyScope;
            public string ApplySkillTag;
            public string ApplySkillID;
            public string TriggerType;
            public float TriggerChance;
            public float TriggerCoolTime;
            public float BuffDuration;
            public int StackLimit;
            public string PassivePrefabPath;

            public SkillPassiveData ToSkillPassiveData()
            {
                var data = new SkillPassiveData();
                data.PassiveEffectType = PassiveEffectType;
                data.TargetStat = TargetStat;
                data.ModifyType = ModifyType;
                data.ModifyValue = ModifyValue;
                data.ApplyScope = ApplyScope;
                data.ApplySkillTag = ApplySkillTag;
                data.ApplySkillID = ApplySkillID;
                data.TriggerType = TriggerType;
                data.TriggerChance = TriggerChance;
                data.TriggerCoolTime = TriggerCoolTime;
                data.BuffDuration = BuffDuration;
                data.StackLimit = StackLimit;
                data.PassivePrefabPath = PassivePrefabPath;
                return data;
            }

            public static SerializedPassiveData FromSkillPassiveData(SkillPassiveData data)
            {
                return new SerializedPassiveData
                {
                    PassiveEffectType = data.PassiveEffectType,
                    TargetStat = data.TargetStat,
                    ModifyType = data.ModifyType,
                    ModifyValue = data.ModifyValue,
                    ApplyScope = data.ApplyScope,
                    ApplySkillTag = data.ApplySkillTag,
                    ApplySkillID = data.ApplySkillID,
                    TriggerType = data.TriggerType,
                    TriggerChance = data.TriggerChance,
                    TriggerCoolTime = data.TriggerCoolTime,
                    BuffDuration = data.BuffDuration,
                    StackLimit = data.StackLimit,
                    PassivePrefabPath = data.PassivePrefabPath
                };
            }
        }

        [Serializable]
        public struct SerializedLevelUpEntry
        {
            public int SkillLevel;
            public string ParamName;
            public ValueType ValueType;
            public float ParamValue;
            public string ConditionType;
            public string ConditionValue;
            public string UITextKey;
        }
    }
}
