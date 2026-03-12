using System;
using System.Collections.Generic;
using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Repository.Csv
{
    public class CsvSkillRepository : ISkillRepository
    {
        private static readonly Dictionary<string, ESkillType> s_skillTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["0"] = ESkillType.ActiveAttack,
            ["ActiveAttack"] = ESkillType.ActiveAttack,
            ["1"] = ESkillType.Passive,
            ["Passive"] = ESkillType.Passive
        };

        private static readonly Dictionary<string, Data.ValueType> s_valueTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["0"] = Data.ValueType.Set,
            ["Set"] = Data.ValueType.Set,
            ["1"] = Data.ValueType.Add,
            ["Add"] = Data.ValueType.Add,
            ["2"] = Data.ValueType.Mult,
            ["Mult"] = Data.ValueType.Mult
        };

        private readonly Dictionary<string, SkillSpec> _skills = new();
        private readonly List<SkillSpec> _allSkills = new();
        private readonly Dictionary<string, List<SkillLevelUpData>> _levelUps = new();

        public CsvSkillRepository(string skillCsvText, string levelUpCsvText)
        {
            ParseSkills(skillCsvText);
            ParseLevelUps(levelUpCsvText);
        }

        public SkillSpec GetById(string id)
        {
            return _skills.TryGetValue(id, out var spec) ? spec : null;
        }

        public IReadOnlyList<SkillSpec> GetAll()
        {
            return _allSkills;
        }

        public IReadOnlyList<SkillLevelUpData> GetLevelUps(string skillId)
        {
            if (_levelUps.TryGetValue(skillId, out var list))
            {
                return list;
            }

            return Array.Empty<SkillLevelUpData>();
        }

        private void ParseSkills(string csvText)
        {
            var rows = CsvParser.Parse(csvText);
            if (rows.Count < 2)
            {
                return;
            }

            string[] headers = rows[0];

            CsvParser.ValidateRequiredColumns(headers, "Skill", "SkillID", "SkillType");

            int idxId = CsvParser.FindColumn(headers, "SkillID");
            int idxName = CsvParser.FindColumn(headers, "SkillName");
            int idxType = CsvParser.FindColumn(headers, "SkillType");
            int idxSubType = CsvParser.FindColumn(headers, "SkillSubType");
            int idxMaxLevel = CsvParser.FindColumn(headers, "MaxLevel");
            int idxGroup = CsvParser.FindColumn(headers, "SkillGroup");
            int idxGrade = CsvParser.FindColumn(headers, "Grade");
            int idxRate = CsvParser.FindColumn(headers, "Rate");
            int idxLinked = CsvParser.FindColumn(headers, "LinkedEffectGroupID");
            int idxDesc = CsvParser.FindColumn(headers, "DescKey");
            int idxIcon = CsvParser.FindColumn(headers, "IconPath");
            int idxPrefab = CsvParser.FindColumn(headers, "PrefabPath");
            int idxCastVfx = CsvParser.FindColumn(headers, "CastVFXPath");
            int idxHitVfx = CsvParser.FindColumn(headers, "HitVFXPath");
            int idxSfx = CsvParser.FindColumn(headers, "SFXPath");
            int idxWeaponReq = CsvParser.FindColumn(headers, "WeaponTagReq");

            // 공격/패시브 데이터용 헤더 인덱스를 한 번만 계산.
            var attackIndices = BuildAttackIndices(headers);
            var passiveIndices = BuildPassiveIndices(headers);

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string id = CsvParser.GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string typeStr = CsvParser.GetField(cols, idxType);
                var skillType = ParseSkillType(typeStr);

                var spec = new SkillSpec
                {
                    Id = id,
                    Name = CsvParser.GetField(cols, idxName),
                    ESkillType = skillType,
                    SubType = CsvParser.GetField(cols, idxSubType),
                    MaxLevel = CsvParser.ParseInt(CsvParser.GetField(cols, idxMaxLevel), 5),
                    SkillGroup = CsvParser.ParseInt(CsvParser.GetField(cols, idxGroup)),
                    Grade = CsvParser.GetField(cols, idxGrade),
                    Rate = CsvParser.ParseFloat(CsvParser.GetField(cols, idxRate)),
                    LinkedEffectGroupIds = CsvParser.ParseStringArray(CsvParser.GetField(cols, idxLinked), ';'),
                    DescKey = CsvParser.GetField(cols, idxDesc),
                    IconPath = CsvParser.GetField(cols, idxIcon),
                    PrefabPath = CsvParser.GetField(cols, idxPrefab),
                    CastVFXPath = CsvParser.GetField(cols, idxCastVfx),
                    HitVFXPath = CsvParser.GetField(cols, idxHitVfx),
                    SFXPath = CsvParser.GetField(cols, idxSfx),
                    WeaponTagReq = CsvParser.GetField(cols, idxWeaponReq)
                };

                if (skillType == ESkillType.ActiveAttack)
                {
                    spec.AttackData = ParseAttackData(attackIndices, cols);
                }
                else
                {
                    spec.PassiveData = ParsePassiveData(passiveIndices, cols);
                }

                _skills[id] = spec;
                _allSkills.Add(spec);
            }
        }

        private void ParseLevelUps(string csvText)
        {
            if (string.IsNullOrEmpty(csvText))
            {
                return;
            }

            var rows = CsvParser.Parse(csvText);
            if (rows.Count < 2)
            {
                return;
            }

            string[] headers = rows[0];

            CsvParser.ValidateRequiredColumns(headers, "SkillLevelUp", "SkillID", "SkillLevel", "ParamName");

            int idxSkillId = CsvParser.FindColumn(headers, "SkillID");
            int idxLevel = CsvParser.FindColumn(headers, "SkillLevel");
            int idxParam = CsvParser.FindColumn(headers, "ParamName");
            int idxValueType = CsvParser.FindColumn(headers, "ValueType");
            int idxValue = CsvParser.FindColumn(headers, "ParamValue");
            int idxCondType = CsvParser.FindColumn(headers, "ConditionType");
            int idxCondValue = CsvParser.FindColumn(headers, "ConditionValue");
            int idxUITextKey = CsvParser.FindColumn(headers, "UITextKey");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string skillId = CsvParser.GetField(cols, idxSkillId);
                if (string.IsNullOrEmpty(skillId))
                {
                    continue;
                }

                int level = CsvParser.ParseInt(CsvParser.GetField(cols, idxLevel), 1);
                string paramName = CsvParser.GetField(cols, idxParam);
                string valueTypeStr = CsvParser.GetField(cols, idxValueType);
                float paramValue = CsvParser.ParseFloat(CsvParser.GetField(cols, idxValue));
                string condType = CsvParser.GetField(cols, idxCondType);
                string condValue = CsvParser.GetField(cols, idxCondValue);
                string uiTextKey = CsvParser.GetField(cols, idxUITextKey);

                var valueType = ParseValueType(valueTypeStr);

                var mod = new ParamModification(
                    paramName, valueType, paramValue,
                    string.IsNullOrEmpty(condType) ? null : condType,
                    string.IsNullOrEmpty(condValue) ? null : condValue);

                if (!_levelUps.TryGetValue(skillId, out var list))
                {
                    list = new List<SkillLevelUpData>();
                    _levelUps[skillId] = list;
                }

                var existing = list.Find(x => x.SkillLevel == level);
                if (existing != null)
                {
                    existing.Modifications.Add(mod);
                }
                else
                {
                    list.Add(new SkillLevelUpData(
                        skillId, level, new List<ParamModification> { mod },
                        string.IsNullOrEmpty(uiTextKey) ? null : uiTextKey));
                }
            }

            // 레벨 순 정렬.
            foreach (var list in _levelUps.Values)
            {
                list.Sort((a, b) => a.SkillLevel.CompareTo(b.SkillLevel));
            }
        }

        private struct AttackColumnIndices
        {
            public int SkillCoef, BaseCoolTime, CritEnabled, LifestealEnabled;
            public int BaseRange, BaseRadius, BaseKnockback;
            public int PierceCount, ChainCount, BounceCount;
            public int BaseProj, BaseProjSpeed, FirePattern;
            public int BaseDuration, TickInterval, TickCoef;
            public int MaxStack, StackRule;
        }

        private static AttackColumnIndices BuildAttackIndices(string[] headers)
        {
            return new AttackColumnIndices
            {
                SkillCoef = CsvParser.FindColumn(headers, "SkillCoef"),
                BaseCoolTime = CsvParser.FindColumn(headers, "BaseCoolTime"),
                CritEnabled = CsvParser.FindColumn(headers, "CritEnabled"),
                LifestealEnabled = CsvParser.FindColumn(headers, "LifestealEnabled"),
                BaseRange = CsvParser.FindColumn(headers, "BaseRange"),
                BaseRadius = CsvParser.FindColumn(headers, "BaseRadius"),
                BaseKnockback = CsvParser.FindColumn(headers, "BaseKnockback"),
                PierceCount = CsvParser.FindColumn(headers, "PierceCount"),
                ChainCount = CsvParser.FindColumn(headers, "ChainCount"),
                BounceCount = CsvParser.FindColumn(headers, "BounceCount"),
                BaseProj = CsvParser.FindColumn(headers, "BaseProj"),
                BaseProjSpeed = CsvParser.FindColumn(headers, "BaseProjSpeed"),
                FirePattern = CsvParser.FindColumn(headers, "FirePattern"),
                BaseDuration = CsvParser.FindColumn(headers, "BaseDuration"),
                TickInterval = CsvParser.FindColumn(headers, "TickInterval"),
                TickCoef = CsvParser.FindColumn(headers, "TickCoef"),
                MaxStack = CsvParser.FindColumn(headers, "MaxStack"),
                StackRule = CsvParser.FindColumn(headers, "StackRule")
            };
        }

        private static SkillAttackData ParseAttackData(AttackColumnIndices idx, string[] cols)
        {
            var data = new SkillAttackData();

            data.SkillCoef = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.SkillCoef));
            data.BaseCoolTime = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BaseCoolTime));
            data.CritEnabled = CsvParser.ParseBool(CsvParser.GetField(cols, idx.CritEnabled));
            data.LifestealEnabled = CsvParser.ParseBool(CsvParser.GetField(cols, idx.LifestealEnabled));
            data.BaseRange = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BaseRange));
            data.BaseRadius = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BaseRadius));
            data.BaseKnockback = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BaseKnockback));
            data.PierceCount = CsvParser.ParseInt(CsvParser.GetField(cols, idx.PierceCount));
            data.ChainCount = CsvParser.ParseInt(CsvParser.GetField(cols, idx.ChainCount));
            data.BounceCount = CsvParser.ParseInt(CsvParser.GetField(cols, idx.BounceCount));
            data.BaseProj = CsvParser.ParseInt(CsvParser.GetField(cols, idx.BaseProj), 1);
            data.BaseProjSpeed = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BaseProjSpeed));
            data.FirePattern = CsvParser.GetField(cols, idx.FirePattern);
            data.BaseDuration = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BaseDuration));
            data.TickInterval = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.TickInterval));
            data.TickCoef = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.TickCoef));
            data.MaxStack = CsvParser.ParseInt(CsvParser.GetField(cols, idx.MaxStack), 1);
            data.StackRule = CsvParser.ParseInt(CsvParser.GetField(cols, idx.StackRule));

            return data;
        }

        private struct PassiveColumnIndices
        {
            public int PassiveEffectType, TargetStat, ModifyType, ModifyValue;
            public int ApplyScope, ApplySkillTag, ApplySkillID;
            public int TriggerType, TriggerChance, TriggerCoolTime;
            public int BuffDuration, StackLimit, PassivePrefabPath;
        }

        private static PassiveColumnIndices BuildPassiveIndices(string[] headers)
        {
            return new PassiveColumnIndices
            {
                PassiveEffectType = CsvParser.FindColumn(headers, "PassiveEffectType"),
                TargetStat = CsvParser.FindColumn(headers, "TargetStat"),
                ModifyType = CsvParser.FindColumn(headers, "ModifyType"),
                ModifyValue = CsvParser.FindColumn(headers, "ModifyValue"),
                ApplyScope = CsvParser.FindColumn(headers, "ApplyScope"),
                ApplySkillTag = CsvParser.FindColumn(headers, "ApplySkillTag"),
                ApplySkillID = CsvParser.FindColumn(headers, "ApplySkillID"),
                TriggerType = CsvParser.FindColumn(headers, "TriggerType"),
                TriggerChance = CsvParser.FindColumn(headers, "TriggerChance"),
                TriggerCoolTime = CsvParser.FindColumn(headers, "TriggerCoolTime"),
                BuffDuration = CsvParser.FindColumn(headers, "BuffDuration"),
                StackLimit = CsvParser.FindColumn(headers, "StackLimit"),
                PassivePrefabPath = CsvParser.FindColumn(headers, "PassivePrefabPath")
            };
        }

        private static SkillPassiveData ParsePassiveData(PassiveColumnIndices idx, string[] cols)
        {
            var data = new SkillPassiveData();

            data.PassiveEffectType = CsvParser.GetField(cols, idx.PassiveEffectType);
            data.TargetStat = CsvParser.GetField(cols, idx.TargetStat);
            data.ModifyType = CsvParser.GetField(cols, idx.ModifyType);
            data.ModifyValue = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.ModifyValue));
            data.ApplyScope = CsvParser.GetField(cols, idx.ApplyScope);
            data.ApplySkillTag = CsvParser.GetField(cols, idx.ApplySkillTag);
            data.ApplySkillID = CsvParser.GetField(cols, idx.ApplySkillID);
            data.TriggerType = CsvParser.GetField(cols, idx.TriggerType);
            data.TriggerChance = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.TriggerChance));
            data.TriggerCoolTime = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.TriggerCoolTime));
            data.BuffDuration = CsvParser.ParseFloat(CsvParser.GetField(cols, idx.BuffDuration));
            data.StackLimit = CsvParser.ParseInt(CsvParser.GetField(cols, idx.StackLimit));
            data.PassivePrefabPath = CsvParser.GetField(cols, idx.PassivePrefabPath);

            return data;
        }

        private static ESkillType ParseSkillType(string value)
        {
            if (s_skillTypeMap.TryGetValue(value, out var type))
            {
                return type;
            }

            UnityEngine.Debug.LogWarning($"[CsvSkillRepository] 알 수 없는 SkillType '{value}', ActiveAttack으로 처리.");
            return ESkillType.ActiveAttack;
        }

        private static Data.ValueType ParseValueType(string value)
        {
            if (s_valueTypeMap.TryGetValue(value, out var type))
            {
                return type;
            }

            UnityEngine.Debug.LogWarning($"[CsvSkillRepository] 알 수 없는 ValueType '{value}', Add로 처리.");
            return Data.ValueType.Add;
        }
    }
}
