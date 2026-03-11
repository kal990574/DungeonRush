using System.Collections.Generic;
using DungeonRush.Stats.Data;

namespace DungeonRush.Stats.Repository
{
    public interface ISkillRepository
    {
        SkillSpec GetById(string id);
        IReadOnlyList<SkillSpec> GetAll();
        IReadOnlyList<SkillLevelUpData> GetLevelUps(string skillId);
    }
}
