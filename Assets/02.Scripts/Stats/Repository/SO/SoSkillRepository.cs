using System;
using System.Collections.Generic;
using DungeonRush.Stats.Data;
using DungeonRush.Stats.Data.SO;

namespace DungeonRush.Stats.Repository.SO
{
    public class SoSkillRepository : ISkillRepository
    {
        private readonly Dictionary<string, SkillSpec> _skills = new();
        private readonly List<SkillSpec> _allSkills = new();
        private readonly Dictionary<string, List<SkillLevelUpData>> _levelUps = new();

        public SoSkillRepository(SkillSpecSO[] skillSOs)
        {
            if (skillSOs == null)
            {
                throw new ArgumentNullException(nameof(skillSOs));
            }

            foreach (var so in skillSOs)
            {
                if (so == null)
                {
                    continue;
                }

                var spec = so.ToSkillSpec();
                _skills[spec.Id] = spec;
                _allSkills.Add(spec);

                var levelUps = so.ToLevelUpDataList();
                if (levelUps.Count > 0)
                {
                    _levelUps[spec.Id] = levelUps;
                }
            }
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
    }
}
