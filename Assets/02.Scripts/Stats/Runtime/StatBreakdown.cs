using System.Collections.Generic;

namespace DungeonRush.Stats.Runtime
{
    public class StatBreakdown
    {
        public string StatKey;
        public float BaseValue;
        public float FinalValue;
        public List<StatBreakdownEntry> Entries;

        public StatBreakdown(string statKey, float baseValue, float finalValue, List<StatBreakdownEntry> entries)
        {
            StatKey = statKey;
            BaseValue = baseValue;
            FinalValue = finalValue;
            Entries = entries;
        }
    }
}
