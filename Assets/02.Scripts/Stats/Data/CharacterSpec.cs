namespace DungeonRush.Stats.Data
{
    public class CharacterSpec
    {
        public string Id;
        public string Name;
        public int Grade;
        public string ModelPrefab;
        public BaseStats Stats;
        public string StartActiveSkillId;
        public string StartPassiveSkillId;
        public string WeaponTypeTag;
        public string CharacterTraitId;

        public CharacterSpec(
            string id,
            string name,
            int grade,
            string modelPrefab,
            BaseStats stats,
            string startActiveSkillId,
            string startPassiveSkillId,
            string weaponTypeTag,
            string characterTraitId)
        {
            Id = id;
            Name = name;
            Grade = grade;
            ModelPrefab = modelPrefab;
            Stats = stats;
            StartActiveSkillId = startActiveSkillId;
            StartPassiveSkillId = startPassiveSkillId;
            WeaponTypeTag = weaponTypeTag;
            CharacterTraitId = characterTraitId;
        }
    }
}
