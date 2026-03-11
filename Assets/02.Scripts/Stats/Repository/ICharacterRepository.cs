using System.Collections.Generic;
using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Repository
{
    public interface ICharacterRepository
    {
        CharacterSpec GetById(string id);
        IReadOnlyList<CharacterSpec> GetAll();
    }
}
