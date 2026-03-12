namespace DungeonRush.Stats.Data
{
    public enum ESkillType
    {
        ActiveAttack,
        Passive
    }

    public class SkillSpec
    {
        public string Id;
        public string Name;
        public ESkillType SkillType;
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

        public bool IsAttack => SkillType == ESkillType.ActiveAttack;
        public bool IsPassive => SkillType == ESkillType.Passive;
    }
}
