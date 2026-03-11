namespace DungeonRush.Stats.Data
{
    public enum SkillCategory
    {
        Attack,
        Passive
    }

    public class SkillSpec
    {
        public string Id;
        public string Name;
        public string Description;
        public SkillCategory Category;
        public int MaxLevel;
        public SkillAttackData AttackData;
        public SkillPassiveData PassiveData;

        public SkillSpec(
            string id,
            string name,
            string description,
            SkillCategory category,
            int maxLevel,
            SkillAttackData attackData,
            SkillPassiveData passiveData)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            MaxLevel = maxLevel;
            AttackData = attackData;
            PassiveData = passiveData;
        }

        public bool IsAttack => Category == SkillCategory.Attack;
        public bool IsPassive => Category == SkillCategory.Passive;
    }
}
