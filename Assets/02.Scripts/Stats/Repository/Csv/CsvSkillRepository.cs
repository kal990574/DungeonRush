using System;
using System.Collections.Generic;
using System.Globalization;
using DungeonRush.Stats.Data;
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

            // 공통 헤더 인덱스 탐색.
            int idxId = FindColumn(headers, "Id");
            int idxName = FindColumn(headers, "Name");
            int idxDesc = FindColumn(headers, "Description");
            int idxCategory = FindColumn(headers, "Category");
            int idxMaxLevel = FindColumn(headers, "MaxLevel");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                string id = GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string name = GetField(cols, idxName);
                string description = GetField(cols, idxDesc);
                string categoryStr = GetField(cols, idxCategory);
                int maxLevel = ParseInt(GetField(cols, idxMaxLevel), 5);

                var category = categoryStr.Equals("Passive", StringComparison.OrdinalIgnoreCase)
                    ? SkillCategory.Passive
                    : SkillCategory.Attack;

                SkillAttackData attackData = null;
                SkillPassiveData passiveData = null;

                if (category == SkillCategory.Attack)
                {
                    attackData = ParseAttackData(headers, cols);
                }
                else
                {
                    passiveData = ParsePassiveData(headers, cols);
                }

                var spec = new SkillSpec(id, name, description, category, maxLevel, attackData, passiveData);
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
            int idxSkillId = FindColumn(headers, "SkillId");
            int idxLevel = FindColumn(headers, "Level");
            int idxParam = FindColumn(headers, "ParamName");
            int idxValueType = FindColumn(headers, "ValueType");
            int idxValue = FindColumn(headers, "Value");

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
                float value = ParseFloat(GetField(cols, idxValue));

                var valueType = valueTypeStr.Equals("Percent", StringComparison.OrdinalIgnoreCase)
                    ? ValueType.Percent
                    : ValueType.Flat;

                var mod = new ParamModification(paramName, valueType, value);

                // 동일 (SkillId, Level) 그룹에 수정 항목 추가.
                if (!_levelUps.TryGetValue(skillId, out var list))
                {
                    list = new List<SkillLevelUpData>();
                    _levelUps[skillId] = list;
                }

                var existing = list.Find(x => x.Level == level);
                if (existing != null)
                {
                    existing.Modifications.Add(mod);
                }
                else
                {
                    list.Add(new SkillLevelUpData(skillId, level, new List<ParamModification> { mod }));
                }
            }

            // 레벨 순 정렬.
            foreach (var list in _levelUps.Values)
            {
                list.Sort((a, b) => a.Level.CompareTo(b.Level));
            }
        }

        private static SkillAttackData ParseAttackData(string[] headers, string[] cols)
        {
            var data = new SkillAttackData();

            for (int c = 0; c < headers.Length && c < cols.Length; c++)
            {
                string header = headers[c].Trim();
                string value = cols[c].Trim();

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                try
                {
                    data.SetParam(header, ParseFloat(value));
                }
                catch (ArgumentException)
                {
                    // 공통 헤더이거나 알 수 없는 파라미터는 무시.
                }
            }

            return data;
        }

        private static SkillPassiveData ParsePassiveData(string[] headers, string[] cols)
        {
            var data = new SkillPassiveData();

            for (int c = 0; c < headers.Length && c < cols.Length; c++)
            {
                string header = headers[c].Trim();
                string value = cols[c].Trim();

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (header == "TargetStat")
                {
                    data.TargetStat = value;
                    continue;
                }

                try
                {
                    data.SetParam(header, ParseFloat(value));
                }
                catch (ArgumentException)
                {
                    // 공통 헤더이거나 알 수 없는 파라미터는 무시.
                }
            }

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
    }
}
