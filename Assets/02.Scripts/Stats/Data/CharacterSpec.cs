namespace DungeonRush.Stats.Data
{
    public class CharacterSpec
    {
        public string Id;
        public string Name;
        public string Type;
        public BaseStats Stats;

        public CharacterSpec(string id, string name, string type, BaseStats stats)
        {
            Id = id;
            Name = name;
            Type = type;
            Stats = stats;
        }
    }
}
