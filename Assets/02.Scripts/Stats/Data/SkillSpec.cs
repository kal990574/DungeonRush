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
        public SkillCategory Category;
        public string SubType;
        public int MaxLevel;
        public int SkillGroup;
        public string Grade;
        public float Rate;
        public string[] LinkedEffectGroupIds;
        public string DescKey;
        public string IconPath;
        public string PrefabPath;
        public string CastVFXPath;
        public string HitVFXPath;
        public string SFXPath;
        public string WeaponTagReq;
        public SkillAttackData AttackData;
        public SkillPassiveData PassiveData;

        public bool IsAttack => Category == SkillCategory.Attack;
        public bool IsPassive => Category == SkillCategory.Passive;
    }
}
