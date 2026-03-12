using System;
using System.Collections.Generic;
using DungeonRush.Stats.Data;
using DungeonRush.Stats.Data.SO;

namespace DungeonRush.Stats.Repository.SO
{
    public class SoCharacterRepository : ICharacterRepository
    {
        private readonly Dictionary<string, CharacterSpec> _characters = new();
        private readonly List<CharacterSpec> _allCharacters = new();

        public SoCharacterRepository(CharacterSpecSO[] characterSOs)
        {
            if (characterSOs == null)
            {
                throw new ArgumentNullException(nameof(characterSOs));
            }

            foreach (var so in characterSOs)
            {
                if (so == null)
                {
                    continue;
                }

                var spec = so.ToCharacterSpec();
                _characters[spec.Id] = spec;
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
