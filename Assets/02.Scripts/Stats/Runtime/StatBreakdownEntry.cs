namespace DungeonRush.Stats.Runtime
{
    public readonly struct StatBreakdownEntry
    {
        public readonly string SourceKey;
        public readonly ModifyType ModifyType;
        public readonly float Value;

        public StatBreakdownEntry(string sourceKey, ModifyType modifyType, float value)
        {
            SourceKey = sourceKey;
            ModifyType = modifyType;
            Value = value;
        }
    }
}
