using System;
using System.Collections.Generic;
using System.Globalization;
using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Repository.Csv
{
    public class CsvCharacterRepository : ICharacterRepository
    {
        private readonly Dictionary<string, CharacterSpec> _characters = new();
        private readonly List<CharacterSpec> _allCharacters = new();

        public CsvCharacterRepository(string csvText)
        {
            var rows = CsvParser.Parse(csvText);
            if (rows.Count < 2)
            {
                return;
            }

            string[] headers = rows[0];

            int idxId = FindColumn(headers, "PlayerID");
            int idxName = FindColumn(headers, "PlayerName");
            int idxGrade = FindColumn(headers, "Grade");
            int idxModel = FindColumn(headers, "PlayerModelPrefab");
            int idxActiveSkill = FindColumn(headers, "StartActiveSkillID");
            int idxPassiveSkill = FindColumn(headers, "StartPassiveSkillID");
            int idxWeaponTag = FindColumn(headers, "WeaponTypeTag");
            int idxTrait = FindColumn(headers, "CharacterTraitID");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];

                string id = GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string name = GetField(cols, idxName);
                int grade = ParseInt(GetField(cols, idxGrade), 0);
                string modelPrefab = GetField(cols, idxModel);
                string activeSkill = GetField(cols, idxActiveSkill);
                string passiveSkill = GetField(cols, idxPassiveSkill);
                string weaponTag = GetField(cols, idxWeaponTag);
                string traitId = GetField(cols, idxTrait);

                var stats = new BaseStats();
                for (int c = 0; c < headers.Length && c < cols.Length; c++)
                {
                    string header = headers[c].Trim();
                    string value = cols[c].Trim();

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                    {
                        try
                        {
                            stats.SetValue(header, parsed);
                        }
                        catch (ArgumentException)
                        {
                            // 스탯 키가 아닌 컬럼은 무시.
                        }
                    }
                }

                var spec = new CharacterSpec(
                    id, name, grade, modelPrefab, stats,
                    activeSkill, passiveSkill, weaponTag, traitId);
                _characters[id] = spec;
                _allCharacters.Add(spec);
            }
        }

        public CharacterSpec GetById(string id)
        {
            return _characters.TryGetValue(id, out var spec) ? spec : null;
        }

        public IReadOnlyList<CharacterSpec> GetAll()
        {
            return _allCharacters;
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
