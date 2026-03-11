using System.Collections.Generic;

namespace DungeonRush.Stats.Data
{
    public class SkillLevelUpData
    {
        public string SkillId;
        public int Level;
        public List<ParamModification> Modifications;

        public SkillLevelUpData(string skillId, int level, List<ParamModification> modifications)
        {
            SkillId = skillId;
            Level = level;
            Modifications = modifications;
        }
    }
}
