namespace DungeonRush.Stats.Runtime
{
    public enum ModifyType
    {
        Flat,
        Percent
    }

    public class StatModifier
    {
        public string SourceKey;
        public string StatKey;
        public ModifyType ModifyType;
        public float Value;

        public StatModifier(string sourceKey, string statKey, ModifyType modifyType, float value)
        {
            SourceKey = sourceKey;
            StatKey = statKey;
            ModifyType = modifyType;
            Value = value;
        }
    }
}
