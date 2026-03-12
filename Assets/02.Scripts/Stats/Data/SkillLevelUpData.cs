using System.Collections.Generic;

namespace DungeonRush.Stats.Data
{
    public class SkillLevelUpData
    {
        public string SkillId;
        public int SkillLevel;
        public List<ParamModification> Modifications;
        public string UITextKey;

        public SkillLevelUpData(string skillId, int skillLevel, List<ParamModification> modifications, string uiTextKey = null)
        {
            SkillId = skillId;
            SkillLevel = skillLevel;
            Modifications = modifications;
            UITextKey = uiTextKey;
        }
    }
}
