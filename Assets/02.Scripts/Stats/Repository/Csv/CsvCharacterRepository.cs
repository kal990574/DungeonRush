using System.Collections.Generic;
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

            CsvParser.ValidateRequiredColumns(headers, "Character", "PlayerID", "PlayerName");

            int idxId = CsvParser.FindColumn(headers, "PlayerID");
            int idxName = CsvParser.FindColumn(headers, "PlayerName");
            int idxGrade = CsvParser.FindColumn(headers, "Grade");
            int idxModel = CsvParser.FindColumn(headers, "PlayerModelPrefab");
            int idxActiveSkill = CsvParser.FindColumn(headers, "StartActiveSkillID");
            int idxPassiveSkill = CsvParser.FindColumn(headers, "StartPassiveSkillID");
            int idxWeaponTag = CsvParser.FindColumn(headers, "WeaponTypeTag");
            int idxTrait = CsvParser.FindColumn(headers, "CharacterTraitID");

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];

                string id = CsvParser.GetField(cols, idxId);
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string name = CsvParser.GetField(cols, idxName);
                int grade = CsvParser.ParseInt(CsvParser.GetField(cols, idxGrade));
                string modelPrefab = CsvParser.GetField(cols, idxModel);
                string activeSkill = CsvParser.GetField(cols, idxActiveSkill);
                string passiveSkill = CsvParser.GetField(cols, idxPassiveSkill);
                string weaponTag = CsvParser.GetField(cols, idxWeaponTag);
                string traitId = CsvParser.GetField(cols, idxTrait);

                var stats = new BaseStats();
                for (int c = 0; c < headers.Length && c < cols.Length; c++)
                {
                    string header = headers[c].Trim();
                    string value = cols[c].Trim();

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    float parsed = CsvParser.ParseFloat(value);
                    if (parsed == 0f && value != "0")
                    {
                        continue;
                    }

                    // Base 접두사 strip. (예: BaseATK → ATK)
                    string statKey = header.StartsWith("Base", System.StringComparison.Ordinal)
                        ? header.Substring(4)
                        : header;

                    if (stats.HasKey(statKey))
                    {
                        stats.SetValue(statKey, parsed);
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
    }
}
