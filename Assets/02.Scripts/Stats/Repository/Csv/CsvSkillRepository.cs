using System;
using System.Collections.Generic;
using System.Globalization;
using DungeonRush.Stats.Data;
using SkillType = DungeonRush.Stats.Data.SkillType;
using ValueType = DungeonRush.Stats.Data.ValueType;

namespace DungeonRush.Stats.Repository.Csv
{
    public class CsvSkillRepository : ISkillRepository
    {
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

            int idxId = FindColumn(headers, "SkillID");
            int idxName = FindColumn(headers, "SkillName");
            int idxType = FindColumn(headers, "SkillType");
            int idxSubType = FindColumn(headers, "SkillSubType");
            int idxMaxLevel = FindColumn(headers, "MaxLevel");
            int idxGroup = FindColumn(headers, "SkillGroup");
            int idxGrade = FindColumn(headers, "Grade");
            int idxRate = FindColumn(headers, "Rate");
            int idxLinked = FindColumn(headers, "LinkedEffectGroupID");
            int idxDesc = FindColumn(headers, "DescKey");
            int idxIcon = FindColumn(headers, "IconPath");
            int idxPrefab = FindColumn(headers, "PrefabPath");
            int idxCastVfx = FindColumn(headers, "CastVFXPath");
            int idxHitVfx = FindColumn(headers, "HitVFXPath");
            int idxSfx = FindColumn(headers, "SFXPath");
            int idxWeaponReq = FindColumn(headers, "WeaponTagReq");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string id = GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string typeStr = GetField(cols, idxType);
                var skillType = typeStr == "1"
                    ? Data.SkillType.Passive
                    : Data.SkillType.ActiveAttack;

                var spec = new SkillSpec
                {
                    Id = id,
                    Name = GetField(cols, idxName),
                    SkillType = skillType,
                    SubType = GetField(cols, idxSubType),
                    MaxLevel = ParseInt(GetField(cols, idxMaxLevel), 5),
                    SkillGroup = ParseInt(GetField(cols, idxGroup), 0),
                    Grade = GetField(cols, idxGrade),
                    Rate = ParseFloat(GetField(cols, idxRate)),
                    LinkedEffectGroupIds = ParseStringArray(GetField(cols, idxLinked), ';'),
                    DescKey = GetField(cols, idxDesc),
                    IconPath = GetField(cols, idxIcon),
                    PrefabPath = GetField(cols, idxPrefab),
                    CastVFXPath = GetField(cols, idxCastVfx),
                    HitVFXPath = GetField(cols, idxHitVfx),
                    SFXPath = GetField(cols, idxSfx),
                    WeaponTagReq = GetField(cols, idxWeaponReq)
                };

                if (skillType == Data.SkillType.ActiveAttack)
                {
                    spec.AttackData = ParseAttackData(headers, cols);
                }
                else
                {
                    spec.PassiveData = ParsePassiveData(headers, cols);
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
            int idxSkillId = FindColumn(headers, "SkillID");
            int idxLevel = FindColumn(headers, "SkillLevel");
            int idxParam = FindColumn(headers, "ParamName");
            int idxValueType = FindColumn(headers, "ValueType");
            int idxValue = FindColumn(headers, "ParamValue");
            int idxCondType = FindColumn(headers, "ConditionType");
            int idxCondValue = FindColumn(headers, "ConditionValue");
            int idxUITextKey = FindColumn(headers, "UITextKey");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string skillId = GetField(cols, idxSkillId);
                if (string.IsNullOrEmpty(skillId))
                {
                    continue;
                }

                int level = ParseInt(GetField(cols, idxLevel), 1);
                string paramName = GetField(cols, idxParam);
                string valueTypeStr = GetField(cols, idxValueType);
                float paramValue = ParseFloat(GetField(cols, idxValue));
                string condType = GetField(cols, idxCondType);
                string condValue = GetField(cols, idxCondValue);
                string uiTextKey = GetField(cols, idxUITextKey);

                ValueType valueType;
                if (valueTypeStr.Equals("Mult", StringComparison.OrdinalIgnoreCase)
                    || valueTypeStr == "2")
                {
                    valueType = ValueType.Mult;
                }
                else if (valueTypeStr.Equals("Set", StringComparison.OrdinalIgnoreCase)
                    || valueTypeStr == "0")
                {
                    valueType = ValueType.Set;
                }
                else
                {
                    valueType = ValueType.Add;
                }

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

        private static SkillAttackData ParseAttackData(string[] headers, string[] cols)
        {
            var data = new SkillAttackData();

            int idxSkillCoef = FindColumn(headers, "SkillCoef");
            int idxCoolTime = FindColumn(headers, "BaseCoolTime");
            int idxCritEnabled = FindColumn(headers, "CritEnabled");
            int idxLifestealEnabled = FindColumn(headers, "LifestealEnabled");
            int idxRange = FindColumn(headers, "BaseRange");
            int idxRadius = FindColumn(headers, "BaseRadius");
            int idxKnockback = FindColumn(headers, "BaseKnockback");
            int idxPierce = FindColumn(headers, "PierceCount");
            int idxChain = FindColumn(headers, "ChainCount");
            int idxBounce = FindColumn(headers, "BounceCount");
            int idxProj = FindColumn(headers, "BaseProj");
            int idxProjSpeed = FindColumn(headers, "BaseProjSpeed");
            int idxFirePattern = FindColumn(headers, "FirePattern");
            int idxDuration = FindColumn(headers, "BaseDuration");
            int idxTickInterval = FindColumn(headers, "TickInterval");
            int idxTickCoef = FindColumn(headers, "TickCoef");
            int idxMaxStack = FindColumn(headers, "MaxStack");
            int idxStackRule = FindColumn(headers, "StackRule");

            data.SkillCoef = ParseFloat(GetField(cols, idxSkillCoef));
            data.BaseCoolTime = ParseFloat(GetField(cols, idxCoolTime));
            data.CritEnabled = ParseBool(GetField(cols, idxCritEnabled));
            data.LifestealEnabled = ParseBool(GetField(cols, idxLifestealEnabled));
            data.BaseRange = ParseFloat(GetField(cols, idxRange));
            data.BaseRadius = ParseFloat(GetField(cols, idxRadius));
            data.BaseKnockback = ParseFloat(GetField(cols, idxKnockback));
            data.PierceCount = ParseInt(GetField(cols, idxPierce), 0);
            data.ChainCount = ParseInt(GetField(cols, idxChain), 0);
            data.BounceCount = ParseInt(GetField(cols, idxBounce), 0);
            data.BaseProj = ParseInt(GetField(cols, idxProj), 1);
            data.BaseProjSpeed = ParseFloat(GetField(cols, idxProjSpeed));
            data.FirePattern = GetField(cols, idxFirePattern);
            data.BaseDuration = ParseFloat(GetField(cols, idxDuration));
            data.TickInterval = ParseFloat(GetField(cols, idxTickInterval));
            data.TickCoef = ParseFloat(GetField(cols, idxTickCoef));
            data.MaxStack = ParseInt(GetField(cols, idxMaxStack), 1);
            data.StackRule = ParseInt(GetField(cols, idxStackRule), 0);

            return data;
        }

        private static SkillPassiveData ParsePassiveData(string[] headers, string[] cols)
        {
            var data = new SkillPassiveData();

            int idxEffect = FindColumn(headers, "PassiveEffectType");
            int idxTarget = FindColumn(headers, "TargetStat");
            int idxModType = FindColumn(headers, "ModifyType");
            int idxModValue = FindColumn(headers, "ModifyValue");
            int idxScope = FindColumn(headers, "ApplyScope");
            int idxSkillTag = FindColumn(headers, "ApplySkillTag");
            int idxSkillId = FindColumn(headers, "ApplySkillID");
            int idxTrigger = FindColumn(headers, "TriggerType");
            int idxChance = FindColumn(headers, "TriggerChance");
            int idxCoolTime = FindColumn(headers, "TriggerCoolTime");
            int idxBuffDur = FindColumn(headers, "BuffDuration");
            int idxStackLimit = FindColumn(headers, "StackLimit");
            int idxPrefab = FindColumn(headers, "PassivePrefabPath");

            data.PassiveEffectType = GetField(cols, idxEffect);
            data.TargetStat = GetField(cols, idxTarget);
            data.ModifyType = GetField(cols, idxModType);
            data.ModifyValue = ParseFloat(GetField(cols, idxModValue));
            data.ApplyScope = GetField(cols, idxScope);
            data.ApplySkillTag = GetField(cols, idxSkillTag);
            data.ApplySkillID = GetField(cols, idxSkillId);
            data.TriggerType = GetField(cols, idxTrigger);
            data.TriggerChance = ParseFloat(GetField(cols, idxChance));
            data.TriggerCoolTime = ParseFloat(GetField(cols, idxCoolTime));
            data.BuffDuration = ParseFloat(GetField(cols, idxBuffDur));
            data.StackLimit = ParseInt(GetField(cols, idxStackLimit), 0);
            data.PassivePrefabPath = GetField(cols, idxPrefab);

            return data;
        }

        private static int FindColumn(string[] headers, string name)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Trim().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static string GetField(string[] cols, int index)
        {
            if (index >= 0 && index < cols.Length)
            {
                return cols[index].Trim();
            }

            return string.Empty;
        }

        private static float ParseFloat(string value)
        {
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }

            return 0f;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        private static bool ParseBool(string value)
        {
            return value.Equals("TRUE", StringComparison.OrdinalIgnoreCase);
        }

        private static string[] ParseStringArray(string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<string>();
            }

            string[] parts = value.Split(separator);
            var result = new List<string>();
            foreach (string part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                {
                    result.Add(trimmed);
                }
            }

            return result.ToArray();
        }
    }
}
