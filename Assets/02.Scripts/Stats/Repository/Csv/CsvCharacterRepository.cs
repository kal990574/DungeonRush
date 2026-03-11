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

            for (int r = 1; r < rows.Count; r++)
            {
                string[] cols = rows[r];
                if (cols.Length < 4)
                {
                    continue;
                }

                string id = cols[0].Trim();
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                string name = cols[1].Trim();
                string type = cols[2].Trim();

                var stats = new BaseStats();
                for (int c = 3; c < headers.Length && c < cols.Length; c++)
                {
                    string header = StripBasePrefix(headers[c].Trim());
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
                            // 알 수 없는 스탯 키는 무시.
                        }
                    }
                }

                var spec = new CharacterSpec(id, name, type, stats);
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

        private static string StripBasePrefix(string header)
        {
            if (header.StartsWith("Base", StringComparison.Ordinal) && header.Length > 4)
            {
                return header.Substring(4);
            }

            return header;
        }
    }
}
